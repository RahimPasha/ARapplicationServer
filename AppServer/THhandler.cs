using System.Net;
using System.Collections.Specialized;
using ARApplicationServer.App_Code;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Web;

namespace ARApplicationServer
{
    public static class THhandler
    {
        public static string Register()
        {

            string Response = "";
            int ID = -1;
            NameValueCollection queryString = new NameValueCollection { { "server", Global.ServerName }, { "id", Global.ServerID } };
            using (WebClient request = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress + "/server/register?server={0}&id={1}", Global.ServerName, Global.ServerID);
                Response = request.DownloadString(uriAdress);
                if (Response.Contains("Identifire:"))
                {
                    Response = Response.Replace("Identifire:", "");
                    Response = Response.Replace("\"", "");
                    int.TryParse(Response, out ID);
                }
            }
            if (ID > 0)
            {
                Global.xDoc.SelectSingleNode("Server/Registered").InnerText = "True";
                Global.xDoc.SelectSingleNode("Server/Identifier").InnerText = ID.ToString();
                Global.xDoc.Save(Global.Dfile.FullName);
                return "Registration was successful" + "<br />";
            }

            return "Registration: failed" + "<br />" + "Hub replied:" + Response + "<br />";
        }

        public static string Upload()
        {
            string RegisterationReply = "";
            string UploadReplyxml = "";
            string UploadReplydat = "";
            RegisterationReply = Register();
            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress +
                    "/Target/Upload?Identifier={0}&ID={1}&TargetName={2}",
                    Global.ServerID, Global.Identifier, Global.OutgoingTargetName);

                foreach (string s in Global.Tags)
                    uriAdress += "&Tags[]=" + s;

                var Ufile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.outgoingDatabase +
                    "/" + "shared.xml"));
                UploadReplyxml = Encoding.UTF8.GetString(client.UploadFile(uriAdress, "POST", Ufile.FullName));
            }
            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress +
                    "/Target/Upload?Identifier={0}&ID={1}&TargetName={2}",
                    Global.ServerID, Global.Identifier, Global.OutgoingTargetName);

                foreach (string s in Global.Tags)
                    uriAdress += "&Tags[]=" + s;

                var Ufile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.outgoingDatabase +
                    "/" + "shared.dat"));
                UploadReplydat = Encoding.UTF8.GetString(client.UploadFile(uriAdress, "POST", Ufile.FullName));
            }
            return "Registration: " + RegisterationReply + "<br />" + "Upload for xml: " + UploadReplyxml + "<br />" +
                "<br />" + "Upload for dat: " + UploadReplydat + "<br />";
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