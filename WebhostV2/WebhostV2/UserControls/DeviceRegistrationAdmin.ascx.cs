using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.AccountManagement;

namespace WebhostV2.UserControls
{
    public partial class DeviceRegistrationAdmin : LoggingUserControl
    {
        protected List<DeviceRegstrationTableRow> TableRows
        {
            get
            {
                if(Session["dev_reg_table_rows"] == null)
                {
                    List<DeviceRegstrationTableRow> theRows = new List<DeviceRegstrationTableRow>(); 
                    
                    using (WebhostEntities db = new WebhostEntities())
                    {
                        List<int> requests = db.RegistrationRequests.Where(req => !req.RequestCompleted).Select(req => req.id).ToList();
                        foreach (int id in requests)
                        {
                            theRows.Add(new DeviceRegstrationTableRow(id));
                        }
                    }
                    Session["dev_reg_table_rows"] = theRows;
                    return theRows;
                }

                return (List<DeviceRegstrationTableRow>)Session["dev_reg_table_rows"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrationRequestTable.Rows.Clear();
            RegistrationRequestTable.Rows.Add(DeviceRegstrationTableRow.HeaderRow);
            RegistrationRequestTable.Rows.AddRange(TableRows.ToArray());
        }

        protected void GenerateImportBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<WebhostMySQLConnection.RegistrationRequest> StudentRequests = new List<WebhostMySQLConnection.RegistrationRequest>();
                List<WebhostMySQLConnection.RegistrationRequest> ProctorRequests = new List<WebhostMySQLConnection.RegistrationRequest>();
                foreach(TableRow row in RegistrationRequestTable.Rows)
                {
                    if (!(row is DeviceRegstrationTableRow)) 
                        continue;

                    DeviceRegstrationTableRow reg = (DeviceRegstrationTableRow)row;
                    WebhostMySQLConnection.RegistrationRequest request = db.RegistrationRequests.Where(req => req.id == reg.RegistrationId).Single();
                    request.RequestDenied = reg.Rejected;
                    if (reg.Rejected)
                    {
                        request.RequestCompleted = true;
                        db.SaveChanges();
                        continue;
                    }

                    if(reg.isProctor)
                    {
                        ProctorRequests.Add(request);
                    }
                    else
                    {
                        StudentRequests.Add(request);
                    }
                }

                CSV csv = new CSV();

                List<String> proctorIps = DeviceRegistration.GetRangeOfFreeIPs(DeviceRegistration.ProctorSubnet, ProctorRequests.Count);
                List<String> studentIps = DeviceRegistration.GetRangeOfFreeIPs(DeviceRegistration.StudentSubnet, StudentRequests.Count);

                if(ProctorRequests.Count > 0)
                    for (int i = 0; i < proctorIps.Count; i++)
                    {
                        String ip = proctorIps[i];
                        WebhostMySQLConnection.RegistrationRequest proctor = ProctorRequests[i];
                        proctor.RequestCompleted = true;
                        csv.Add(new Dictionary<string, string>()
                            {
                                {"ScopeId",DeviceRegistration.ProctorSubnet},
                                {"IPAddress", ip},
                                {"Name", String.Format("{0}{1}{2}", proctor.Student.FirstName, proctor.Student.LastName, proctor.DeviceType)},
                                {"ClientId", proctor.MacAddress},
                                {"Description", String.Format("{0} {1}:  {2}", proctor.Student.FirstName, proctor.Student.LastName, proctor.DeviceType)}
                            });
                    }
                if(StudentRequests.Count > 0)
                    for (int i = 0; i < studentIps.Count; i++)
                    {
                        String ip = studentIps[i];
                        WebhostMySQLConnection.RegistrationRequest student = StudentRequests[i];
                        student.RequestCompleted = true;
                        csv.Add(new Dictionary<string, string>()
                            {
                                {"ScopeId",DeviceRegistration.StudentSubnet},
                                {"IPAddress", ip},
                                {"Name", String.Format("{0}{1}{2}", student.Student.FirstName, student.Student.LastName, student.DeviceType)},
                                {"ClientId", student.MacAddress},
                                {"Description", String.Format("{0} {1}:  {2}", student.Student.FirstName, student.Student.LastName, student.DeviceType)}
                            });
                    }


                csv.Save(Server.MapPath("~/Temp/dhcp_import.csv"));
                db.SaveChanges();
                Response.Redirect("~/Temp/dhcp_import.csv");
            }
        }
    }
}