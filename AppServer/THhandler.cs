using System.Web;
using System.Xml;
using System.Net;
using System.Collections.Specialized;
using System.Configuration;
namespace ARApplicationServer
{
    public static class THhandler
    {
        public static string TargetHubAddress = ConfigurationManager.AppSettings["HubAddress"].ToString();
        public static string ServerName;
        public static string ServerID;
        private static string TargetsFolder;
        public static string Registered;
        public static string Identifier;

        public static string Register(string serverConfigFile)
        {
            string incomingDatabase;
            string outgoingDatabase;
            System.IO.FileInfo Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(serverConfigFile));
            //string fileName = "ServerA\\Config.xml";
            XmlDocument xDoc = new XmlDocument(); // reading XML documents
            xDoc.Load(Dfile.FullName);
            ServerName = xDoc.SelectSingleNode("Server/Name").InnerText;
            ServerID = xDoc.SelectSingleNode("Server/ID").InnerText;
            Registered = xDoc.SelectSingleNode("Server/Registered").InnerText;
            Identifier = xDoc.SelectSingleNode("Server/Identifier").InnerText;
            TargetsFolder = xDoc.SelectSingleNode("Server/Targets/TargetsFolder").InnerText;
            incomingDatabase = xDoc.SelectSingleNode("Server/Targets/Incoming").InnerText;
            outgoingDatabase = xDoc.SelectSingleNode("Server/Targets/Outgoing").InnerText;
            string Response = "";
            int ID = -1;
            NameValueCollection queryString = new NameValueCollection { { "server", ServerName }, { "id", ServerID } };
            using (WebClient request = new WebClient())
            {
                string uriAdress = string.Format(TargetHubAddress + "/server/register?server={0}&id={1}", ServerName, ServerID);
                Response = request.OpenRead(uriAdress).ToString();
                if (Response.Contains("Identifire:"))
                {
                    Response = Response.Replace("Identifire:", "");
                    int.TryParse(Response, out ID);
                }
            }
            if (ID > 0)
            {
                xDoc.SelectSingleNode("Server/Registered").InnerText = "True";
                xDoc.SelectSingleNode("Server/Identifier").InnerText = ID.ToString();
                xDoc.Save(Dfile.FullName);
                return "Registration was successful";
            }

            return "Registration failed";
        }
            /*Task<string> Registered = registerAsync();
            if(Registered.Result == "approved")
            {
                WebClient client = new WebClient();
                client.DownloadFile("http://localhost:7204/default.aspx?file=second.xml","second.xml");
                client.DownloadFile("http://localhost:7204/default.aspx?file=second.dat", "second.dat");
            }
            */
            /*
            #region Uploading This server's Database to TH
            //TODO: Try and catch is neccessary due to file not found error and other errors

            NameValueCollection queryString = new NameValueCollection { { "request", "register" }, { "server", ServerName }, { "ID", ServerID } };
            Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(outgoingDatabase + "/" + "shared.xml"));
            HttpContext.Current.Response.Write(Dfile.FullName);

            using (WebClient request = new WebClient())
            {
                request.QueryString.Add(queryString);
                request.UploadFile(TargetHubAddress, Dfile.FullName);
            }
            Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(outgoingDatabase + "/" + "shared.dat"));
            using (WebClient request = new WebClient())
            {
                request.QueryString.Add(queryString);
                request.UploadFile(TargetHubAddress, Dfile.FullName);
            }
            #endregion

            #region Downloading Database of other servers from TH
            //TODO: try and catch is needed
            using (WebClient client = new WebClient())
            {
                //client.QueryString.Add("file")
                Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(incomingDatabase + "/" + "shared.xml"));
                HttpContext.Current.Response.Write(TargetHubAddress + "?file=" + Dfile.Name + "&server=" + ServerName + "&ID=" + ServerID + "--" + Dfile.FullName);
                client.DownloadFile(TargetHubAddress + "?file=" + Dfile.FullName + "&server=" + ServerName + "&ID=" + ServerID, Dfile.FullName);
                Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(incomingDatabase + "/" + "shared.dat"));
                client.DownloadFile(TargetHubAddress + "?file=" + Dfile.FullName + "&server=" + ServerName + "&ID=" + ServerID, Dfile.FullName);
            }
            #endregion

        }
            */
        /* private static async Task<string> registerAsync()
         {
             using (var client = new HttpClient())
             {

                 var values = new Dictionary<string, string> { { "ID", ServerID }, { "Name", ServerName } };
                 var content = new FormUrlEncodedContent(values);
                 var response = await client.PostAsync(TargetHubAddress, content);
                 var responseString = await response.Content.ReadAsStringAsync();
                 return response.Content.ToString().ToLower();
             }
         }*/
    }
}