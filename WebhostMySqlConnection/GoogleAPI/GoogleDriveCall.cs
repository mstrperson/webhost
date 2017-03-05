using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhostMySQLConnection.Web;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util.Store;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Google.Apis.Services;

namespace WebhostMySQLConnection.GoogleAPI
{
    public class GoogleDriveCall : GoogleAPICall
    {
        public GoogleDriveCall() : base() { }

        public CSV ListPublicDocuments(String username)
        {
            SetCredential(new[] { DriveService.Scope.Drive, DriveService.Scope.DriveMetadata, DriveService.Scope.DriveAppdata }, username);
            CSV csv = new CSV();
            DriveService service = new DriveService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = "Dublin School Webhost Gmail" });
            FileList fList = service.Files.List().Execute();
            foreach(File file in fList.Items)
            {
                List<Google.Apis.Drive.v2.Data.Permission> permissions = service.Permissions.List(file.Id).Execute().Items.ToList();
                
                foreach(Google.Apis.Drive.v2.Data.Permission permission in permissions.Where(perm => perm.Type.Equals("anyone") || perm.Type.Equals("domain")))
                {
                    Dictionary<String, String> row = new Dictionary<string, string>()
                    {
                        {"user", username},
                        {"file title", file.Title},
                        {"permission", permission.Type},
                        {"file owner", file.Owners.First().DisplayName},
                        {"link", file.AlternateLink}
                    };

                    csv.Add(row);
                }
            }

            return csv;
        }
    }
}
