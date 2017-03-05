using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;
using System.Runtime.Serialization.Json;
using System.IO;

namespace WebhostMySQLConnection.SchoologyAPI
{
    /// <summary>
    /// Each Instance of SchoologyAPICall is only good for one API Call.  If you reuse the same call, Schoology rejects the request.
    /// </summary>
    public class SchoologyAPICall : IDisposable
    {
        #region OAuth Parameters
        private readonly String realm = "Schoology API";
        /// <summary>
        /// Key is generated from http://schoology.dublinschool.org/api
        /// </summary>
        private String oauth_consumer_key; // Get this from Schoology.
        private readonly String oauth_token = ""; // intentionally left blank--not used in two-legged authentication, but required key.
        private String oauth_nonce  // Needs to be generated, and unique to the given timestamp.  PHP Equiv "uniqid()"
        {
            get
            {
                return System.Guid.NewGuid().ToString().Substring(0,13);
            }
        }
        private static String oauth_timestamp  // The current unix timestamp
        {
            get
            {
                return Convert.ToString(DateTime.UtcNow.Subtract(new DateTime(1970, 1,1)).TotalSeconds);
            }
        }
        private readonly String oauth_signature_method = "PLAINTEXT";
        private readonly String oauth_version = "1.0";
        /// <summary>
        /// Secret is generated from http://schoology.dublinschool.org/api
        /// </summary>
        private String oauth_consumer_secret;   //  Get this from Schoology!
        private String oauth_signature
        {
            get
            {
                return oauth_consumer_secret + "%26";
            }
        }
        #endregion

        private String AuthorizationHeaderValue
        {
            get
            {
                return String.Format("OAuth realm=\"{0}\",oauth_consumer_key=\"{1}\",oauth_token=\"{2}\",oauth_nonce=\"{3}\",oauth_timestamp=\"{4}\",oauth_signature_method=\"{5}\",oauth_version=\"{6}\",oauth_signature=\"{7}\"",
                                realm,
                                oauth_consumer_key,
                                oauth_token,
                                oauth_nonce,
                                oauth_timestamp,
                                oauth_signature_method,
                                oauth_version,
                                oauth_signature);
            }
        }

        private String BaseUrl;

        private RestClient client;

        public SchoologyAPICall()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                ExternalApi schoology = db.ExternalApis.Where(api => api.service.Equals("Schoology")).Single();
                BaseUrl = schoology.BaseUrl;
                oauth_consumer_key = schoology.ClientId;
                oauth_consumer_secret = schoology.ClientSecret; 
                
                client = new RestClient(BaseUrl);

                foreach(ApiRequestHeader header in schoology.ApiRequestHeaders)
                {
                    client.AddDefaultHeader(header.Name, header.Value);
                }

                client.AddDefaultHeader("Authorization", AuthorizationHeaderValue);
            }            
        }

        /// <summary>
        /// Get the list of enrollments associated with a specific section in Schoology
        /// </summary>
        /// <param name="section_id">Schoology Section Id</param>
        /// <returns></returns>
        public List<SchoologyEnrollment> GetSectionEnrollments(int section_id)
        {
            List<SchoologyEnrollment> enrollments = new List<SchoologyEnrollment>();

            String resource = string.Format("v1/sections/{0}/enrollments", section_id);

            XMLTree data;
            try
            {
                data = GET(resource);
            }
            catch(Exception ex)
            {
                WebhostEventLog.SchoologyLog.LogWarning("Could not read the endpoint {0}", resource);
                throw new SchoologyAPIException(String.Format("Could not read the endpoint {0}.", resource), ex);
            }

            foreach(XMLTree enrollmentTree in data.ChildTrees.Where(t => t.TagName.Equals("enrollment")).ToList())
            {
                enrollments.Add(new SchoologyEnrollment(section_id, 
                    enrollmentTree.ChildNodes.Where(node => node.TagName.Equals("id")).Select(node => Convert.ToInt32(node.Value)).Single()));
            }

            return enrollments;
        }

        public XMLTree GetEnrollmentXML(int section_id, int enrollment_id)
        {
            String resource = String.Format("v1/sections/{0}/enrollments/{1}",section_id, enrollment_id);
            return GET(resource);
        }

        protected String GETxml(String resource)
        {
            RestRequest request = new RestRequest(resource, Method.GET);
#if DEBUG
           // Console.WriteLine("Sending Request...");
#endif
            RestResponse response = (RestResponse)client.Execute(request);

            if (response == null)
            {
                WebhostEventLog.SchoologyLog.LogError("{0} GET request returned a NULL response.", resource);
                throw new SchoologyAPIException("Null Response");
            }
            switch (response.ResponseStatus)
            {
                case ResponseStatus.Aborted:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} was aborted:  {1}", resource, response.ErrorMessage);
                    throw new SchoologyAPIException("Request was Aborted!" + response.ErrorMessage);
                case ResponseStatus.Error:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} returned an Error:  {1}", resource, response.ErrorMessage);
                    throw new SchoologyAPIException("Error:  " + response.ErrorMessage);
                case ResponseStatus.None:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} returned no response.", resource);
                    throw new SchoologyAPIException("No Response...");
                case ResponseStatus.TimedOut:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} timed out.", resource);
                    throw new SchoologyAPIException("Timed out!");
                default: break;
            }
#if DEBUG
            //Console.WriteLine("Request Completed Successfully.  Headers:");
            //foreach (Parameter header in response.Headers)
            //{
            //    Console.WriteLine("{0} : {1}", header.Name, header.Value);
            //}
#endif
            return response.Content;
        }

        /// <summary>
        /// Delete a course enrollment.
        /// </summary>
        /// <param name="section_id">Schoology Section Id</param>
        /// <param name="enrollment_id">Schoology Section Enrollment Id</param>
        public void DeleteEnrollment(int section_id, int enrollment_id)
        {
            DELETE(String.Format("v1/sections/{0}/enrollments/{1}", section_id, enrollment_id));
        }

        public void DeleteSection(int section_id)
        {
            DELETE(String.Format("v1/sections/{0}", section_id));
        }

        public XMLTree CreateEnrollment(int schoology_section_id, int webhost_id, bool adm)
        {
            String resource = String.Format("v1/sections/{0}/enrollments", schoology_section_id);
            SchoologyEnrollment.CreateEnrollment enr = new SchoologyEnrollment.CreateEnrollment() { uid = webhost_id, admin = adm };
            /*DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(SchoologyEnrollment.CreateEnrollment));
            MemoryStream ms = new MemoryStream();
            js.WriteObject(ms, enr);

            ms.Position = 0;
            
            StreamReader rd = new StreamReader(ms);

            String json = rd.ReadToEnd();
            rd.Close();
            */
            return POST(resource, enr.ToCreateJson());
        }

        public XMLTree CreateItem(ISchoologyCreatable item)
        {
            return POST(item.Resource, item.ToCreateJson());
        }

        public XMLTree ListGradingPeriods()
        {
            return GET("v1/gradingperiods");
        }

        public XMLTree ListUsers()
        {
            return GET("v1/users?start=0&limit=20");
        }

        protected XMLTree POST(String resource, String json)
        {
            RestRequest request = new RestRequest(resource, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddBody(json);

            RestResponse response = (RestResponse)client.Execute(request);
            if (response == null)
            {
                WebhostEventLog.SchoologyLog.LogError("{0} CREATE request returned a NULL response.", resource);
                throw new SchoologyAPIException("Null Response");
            }
            switch (response.ResponseStatus)
            {
                case ResponseStatus.Aborted:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} was aborted:  {1}", resource, response.ErrorMessage);
                    throw new SchoologyAPIException("Request was Aborted!" + response.ErrorMessage);
                case ResponseStatus.Error:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} returned an Error:  {1}", resource, response.ErrorMessage);
                    throw new SchoologyAPIException("Error:  " + response.ErrorMessage);
                case ResponseStatus.None:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} returned no response.", resource);
                    throw new SchoologyAPIException("No Response...");
                case ResponseStatus.TimedOut:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} timed out.", resource);
                    throw new SchoologyAPIException("Timed out!");
                default: break;
            }

            return new XMLTree(response.Content);
        }

        protected void DELETE(String resource)
        {
            RestRequest request = new RestRequest(resource, Method.DELETE);
            RestResponse response = (RestResponse)client.Execute(request);

            if(response == null)
            {
                WebhostEventLog.SchoologyLog.LogError("{0} DELETE request returned a NULL response.", resource);
                throw new SchoologyAPIException("Null Response");
            }

            switch(response.ResponseStatus)
            {
                case ResponseStatus.Completed:
                    WebhostEventLog.SchoologyLog.LogInformation("Successfully deleted endpoint {0}", resource);
                    break;
                case ResponseStatus.Aborted:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} was aborted:  {1}", resource, response.ErrorMessage);
                    throw new SchoologyAPIException("Request was Aborted!" + response.ErrorMessage);
                case ResponseStatus.Error:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} returned an Error:  {1}", resource, response.ErrorMessage);
                    throw new SchoologyAPIException("Error:  " + response.ErrorMessage);
                case ResponseStatus.None:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} returned no response.", resource);
                    throw new SchoologyAPIException("No Response...");
                case ResponseStatus.TimedOut:
                    WebhostEventLog.SchoologyLog.LogError("Request {0} timed out.", resource);
                    throw new SchoologyAPIException("Timed out!");
                default:
                    WebhostEventLog.SchoologyLog.LogWarning("Default case reached in DELETE response status for {0}", resource);
                    break;
            }
        }

        protected XMLTree GET(String resource)
        {
            string xml = GETxml(resource);
            XMLTree tree = new XMLTree(xml);
            String next = "";

            do
            {
                try
                {
                    next = tree.ChildTrees.Where(t => t.TagName.Equals("links")).Single().ChildNodes.Where(n => n.TagName.Equals("next")).Single().Value;
                    next = next.Replace("amp%3Blimit=20&amp;", "");
                    tree.ChildTrees.Remove(tree.ChildTrees.Where(t => t.TagName.Equals("links")).Single());
                    string nxml = "";
                    using(SchoologyAPICall newCall = new SchoologyAPICall())
                    {
                        string rscr = next.Substring(next.IndexOf("v1"));
                        nxml = newCall.GETxml(rscr);
                    }
                    tree = tree.MergeWith(new XMLTree(nxml));
                }
                catch
                {
                    next = "";
                }
            } while (!next.Equals(""));

            return tree;
        }

        public XMLTree GetSectionsOf(int schoology_course_id)
        {
            String resource = String.Format("v1/courses/{0}/sections", schoology_course_id);
            return GET(resource);
        }

        public XMLTree GetCourses()
        {
            String resource = String.Format("v1/courses?start=0&limit=200");
            return GET(resource);
        }

        public XMLTree GetAttendances(int sectionId)
        {
            String resource = String.Format("v1/sections/{0}/attendance", sectionId);
            return GET(resource);
        }
        public XMLTree GetAttendances(int sectionId, DateRange period)
        {
            String start = String.Format("{0}-{3}{1}-{4}{2}", period.Start.Year, period.Start.Month, period.Start.Day, period.Start.Month < 10 ? "0" : "", period.Start.Day < 10 ? "0" : "");
            String end = String.Format("{0}-{3}{1}-{4}{2}", period.End.Year, period.End.Month, period.End.Day, period.End.Month < 10 ? "0" : "", period.End.Day < 10 ? "0" : "");
            String resource = String.Format("v1/sections/{0}/attendance?start={1}&end={2}", sectionId, start, end);
            return GET(resource);
        }

        public void Dispose()
        {
            //client.ClearHandlers();
        }

        public class SchoologyAPIException : Exception
        {
            public SchoologyAPIException(String message, Exception innerException = null)
                : base(message, innerException)
            {
                WebhostEventLog.SchoologyLog.LogError("Schoology API Exception:  {0}", message);
            }
        }
    }
}
