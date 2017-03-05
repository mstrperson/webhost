using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WebhostMySQLConnection.Web;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Admin.Directory.directory_v1.Data;
using System.Text.RegularExpressions;

namespace WebhostMySQLConnection.GoogleAPI
{
    public abstract class GoogleAPICall : IDisposable
    {

        /// <summary>
        /// Certificate issued by the Google Developer Console
        /// console.developers.google.com
        /// 
        /// This Cert is stored in LocalMachine\TrustedPeople
        /// When re-importing or updating, be sure to set the friendly name to "google-pk"
        /// 
        /// On Webhost, the certificate must also be readable by the "IIS AppPool\DefaultAppPool" user.
        /// This is achieved using the winhttpcertcfg.exe tool.
        /// </summary>
        protected X509Certificate2 cert
        {
            get
            {
                State.log.WriteLine("Getting Certificate from local store!");
                X509Store x509store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine);
                x509store.Open(OpenFlags.ReadOnly);
                X509Certificate2 googleCert = null;

                foreach(X509Certificate2 c in x509store.Certificates)
                {
                    if (c.FriendlyName.Equals("google-pk"))
                    {
                        googleCert = c;
                        State.log.WriteLine("Found the Google PK Cert");
                    }
                }
                
                if(googleCert == null)
                {
                    State.log.WriteLine("I didn't find the cert =(");
                    WebhostEventLog.GoogleLog.LogError("Could not find Google API Cert in Local Certificate Store.");
                    throw new GoogleAPIException("Could not find Google API Cert in Local Certificate Store.");
                }

                x509store.Close();
                return googleCert;
            }
        }

        /// <summary>
        /// Set the credential to have access to only the scopes that you need and the user you wish to impersonate.
        /// Adminj is the owner of all relevant domain level items.
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="userToImpersonate"></param>
        public void SetCredential(String[] scopes, String userToImpersonate = "adminj")
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                ExternalApi gapi = db.ExternalApis.Where(api => api.service.Equals("Google")).Single();
                ServiceAccount acct = gapi.ServiceAccounts.Where(a => a.Password.Equals("cert")).Single();
                State.log.WriteLine("Got API info from Webhost database.");

                // It is important that this be initiallized with the correct ServiceAccount Email address associated with the Security Certificate.  
                // Otherwise your requests are rejected.
                credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(acct.Account)
                {
                    User = String.Format("{0}@dublinschool.org", userToImpersonate),
                    Scopes = scopes
                    //This is the list of APIs that we want to be able to access.  Currently only care about Users.
                }.FromCertificate(cert));

                State.log.WriteLine("Credential Recieved.  Connecting service.");
                WebhostEventLog.GoogleLog.LogInformation("Got credentials accepted for impersonating {0}@dublinschool.org.", userToImpersonate);
            }
        }

        protected ServiceAccountCredential credential;

        public GoogleAPICall()
        {
            State.log.WriteLine("GoogleAPICall constructor:  Connecting to database.");
            credential = null;
        }


        public void Dispose()
        {
            State.log.WriteLine("Disposing GoogleAPICall from memory.");
            credential = null;
        }

        public class GoogleAPIException : Exception
        {
            public GoogleAPIException(String message, Exception innerException = null) : base(message, innerException)
            {

            }
        }
    }
}
