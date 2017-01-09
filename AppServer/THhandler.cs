﻿using System.Net;
using System.Collections.Specialized;
using ARApplicationServer.App_Code;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Xml;

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
                string uriAdress = string.Format(Global.TargetHubAddress +
                    "/server/register?server={0}&id={1}", Global.ServerName, Global.ServerID);
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
            string UploadReplychat = "";
            RegisterationReply = Register();
            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress +
                    "/Target/Upload?Identifier={0}&ID={1}&TargetName={2}",
                    Global.ServerID, Global.Identifier, Global.OutgoingTargetName);

                foreach (string s in Global.UploadingTags)
                    uriAdress += "&Tags[]=" + s;

                var Ufile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.outgoingDatabase +
                    "/" + "shared.xml"));
                UploadReplyxml = Encoding.UTF8.GetString(client.UploadFile(uriAdress, "POST", Ufile.FullName));
            }
            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format("{0}/Target/Upload?Identifier={1}&ID={2}&TargetName={3}", Global.TargetHubAddress,
                    Global.ServerID, Global.Identifier, Global.OutgoingTargetName);

                foreach (string s in Global.UploadingTags)
                    uriAdress += "&Tags[]=" + s;

                var Ufile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.outgoingDatabase +
                    "/" + "shared.dat"));
                UploadReplydat = Encoding.UTF8.GetString(client.UploadFile(uriAdress, "POST", Ufile.FullName));
            }
            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format("{0}/Target/Upload?Identifier={1}&ID={2}&TargetName={3}", Global.TargetHubAddress,
                    Global.ServerID, Global.Identifier, Global.OutgoingTargetName);
                foreach (string s in Global.UploadingTags)
                    uriAdress += "&Tags[]=" + s;

                var Ufile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.ChatFolder +
                    "/" + Global.OutgoingTargetName+"_chat.xml"));
                if (Ufile.Exists)
                {
                    //The target is being shared so its shared status should change to true
                    ChangeStatusToShared(Ufile);
                    UploadReplychat = Encoding.UTF8.GetString(client.UploadFile(uriAdress, "POST", Ufile.FullName));
                }
            }
            return "Registration: " + RegisterationReply + "<br />" + "Upload for xml: " + UploadReplyxml + "<br />" +
                "<br />" + "Upload for dat: " + UploadReplydat + "<br />";
        }

        private static void ChangeStatusToShared(System.IO.FileInfo TargetFile)
        {
            XmlDocument xDoc = new XmlDocument(); // reading XML documents
            xDoc.Load(TargetFile.FullName);
            if (xDoc.SelectSingleNode("/TargetChatFile/Shared") != null)
                xDoc.SelectSingleNode("/TargetChatFile/Shared").InnerText = "True";
            else
            {
                XmlElement shared = xDoc.CreateElement("Shared");
                shared.InnerText = "True";
                xDoc.GetElementsByTagName("TargetChatFile")[0].InsertAfter(shared, xDoc.GetElementsByTagName("TargetChatFile")[0].FirstChild);
                xDoc.Save(TargetFile.FullName);
            }
        }

        internal static string Download()
        {
            string RegisterationReply = "";
            string DownloadReplyxml = "";
            string DownloadReplydat = "";
            string targetname = "Stone";
            RegisterationReply = Register();
            List<string> targets = new List<string>(GetTargets(Global.DownloadingTags));
            if(targets.Count==0)
            {
                return "Get Targets: There is no targets with specified tags";
            }
            else
            {
                targetname = targets[0];
            }
            using (WebClient client = new WebClient())
            {
                //client.QueryString.Add("file")
                var Dowfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.incomingDatabase + "/" + targetname + ".xml"));
                string uriAdress = string.Format("{0}/Target/Download?Identifier={1}&ID={2}&TargetName={3}&format={4}",
                    Global.TargetHubAddress, Global.ServerID, Global.Identifier, targetname, "xml");
                client.DownloadFile(uriAdress, Dowfile.FullName);
                DownloadReplyxml = "OK";
                Dowfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.incomingDatabase + "/" + targetname + ".dat"));
                uriAdress = string.Format("{0}/Target/Download?Identifier={1}&ID={2}&TargetName={3}&format={4}", Global.TargetHubAddress,
                    Global.ServerID, Global.Identifier, targetname, "dat");
                client.DownloadFile(uriAdress, Dowfile.FullName);
                DownloadReplydat = "OK";
                //Download Chat File
                Dowfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.ChatFolder + "/" + targetname+"_chat" + ".xml"));
                uriAdress = string.Format("{0}/Target/Download?Identifier={1}&ID={2}&TargetName={3}&format={4}", Global.TargetHubAddress,
                    Global.ServerID, Global.Identifier, targetname + "_chat", "xml");
                client.DownloadFile(uriAdress, Dowfile.FullName);
            }

            return "Registration: " + RegisterationReply + "<br />" + "Download for xml: " + DownloadReplyxml + "<br />" +
                "<br />" + "Download for dat: " + DownloadReplydat + "<br />";
        }

        public static List<string> GetTargets(List<string> tags)
        {

            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress + "/Target/GetTargets?Identifier={0}&ID={1}",
                    Global.ServerID, Global.Identifier);
                foreach (string s in tags )
                    uriAdress += "&Tags[]=" + s;
                return JsonConvert.DeserializeObject<List<string>>(Encoding.Default.GetString(client.DownloadData(uriAdress)));
            }
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

        internal static void SendMessage(string TargetName, string User, string SentMessage)
        {

            using (WebClient client = new WebClient())
            {
                string uriAdress = HttpUtility.UrlEncode(string.Format(Global.TargetHubAddress +
                    "/Target/ForwardMessage?Identifier={0}&ID={1}&TargetName={2}&UserName={3}&SentMessage={4}",
                    Global.ServerID, Global.Identifier, TargetName, User, SentMessage));
                client.OpenWrite(uriAdress);
            }
        }
    }
}