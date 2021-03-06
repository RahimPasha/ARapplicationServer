﻿using System.Net;
using System.Collections.Specialized;
using ARApplicationServer.App_Code;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Xml;
using System.Linq;
using System.Configuration;
using System.Web.Configuration;
using ARApplicationServer.Models;

namespace ARApplicationServer
{
    public static class THhandler
    {
       static  DAL db = new DAL();
        public static string Register()
        {
            Global.Refresh();

            string Response = "";
            int ID = -1;
            using (WebClient request = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress +
                    "/server/register?server={0}&Identifier={1}&Address={2}", Global.ServerName, Global.Identifier, Global.ServerAddress);
                Response = request.DownloadString(uriAdress);
                if (Response.Contains("ID:"))
                {
                    Response = Response.Replace("ID:", "");
                    Response = Response.Replace("\"", "");
                    int.TryParse(Response, out ID);
                }
            }
            if (ID > 0)
            {
                Global.MyServer.Registered = "True";
                Global.MyServer.HubID = ID;
                db.Servers.FirstOrDefault().Registered = "true";
                db.Servers.FirstOrDefault().HubID = ID;
                db.SaveChanges();
                Global.Refresh();


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
            List<string> tags = new List<string>();
            var TargetXMLfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.OutgoingDatabase +
                    "/" + Global.OutgoingTargetName + ".xml"));
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(TargetXMLfile.FullName);
            tags = xDoc.SelectNodes("QCARConfig/Tags/Tag").Cast<XmlNode>().Select(x => x.InnerText).ToList();
            RegisterationReply = Register();
            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress +
                    "/Target/Upload?Identifier={0}&ID={1}&TargetName={2}",
                    Global.Identifier, Global.HubID, Global.OutgoingTargetName);

                foreach (string s in tags)
                    uriAdress += "&Tags[]=" + s;

                UploadReplyxml = Encoding.UTF8.GetString(client.UploadFile(uriAdress, "POST", TargetXMLfile.FullName));
            }
            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format("{0}/Target/Upload?Identifier={1}&ID={2}&TargetName={3}", Global.TargetHubAddress,
                    Global.Identifier, Global.HubID, Global.OutgoingTargetName);

                foreach (string s in tags)
                    uriAdress += "&Tags[]=" + s;

                var Ufile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.OutgoingDatabase +
                    "/" + Global.OutgoingTargetName + ".dat"));
                UploadReplydat = Encoding.UTF8.GetString(client.UploadFile(uriAdress, "POST", Ufile.FullName));
            }
            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format("{0}/Target/Upload?Identifier={1}&ID={2}&TargetName={3}", Global.TargetHubAddress,
                    Global.Identifier, Global.HubID, Global.OutgoingTargetName);
                foreach (string s in tags)
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
            }
            xDoc.Save(TargetFile.FullName);
        }

        internal static string Download(string targetname)
        {
            //TODO: Check for the reply message coming from server before changing the database.
            string RegisterationReply = "";
            string DownloadReplyxml = "";
            string DownloadReplydat = "";
            RegisterationReply = Register();
            using (WebClient client = new WebClient())
            {
                //client.QueryString.Add("file")
                var Dowfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.IncomingDatabase + "/" + targetname + ".xml"));
                string uriAdress = string.Format("{0}/Target/Download?Identifier={1}&ID={2}&TargetName={3}&format={4}",
                    Global.TargetHubAddress, Global.Identifier, Global.HubID, targetname, "xml");
                Target target;
                try
                {
                    client.DownloadFile(uriAdress, Dowfile.FullName);
                    DownloadReplyxml = "OK";
                    List<string> tags = new List<string>();
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(Dowfile.FullName);
                    tags = xDoc.SelectNodes("QCARConfig/Tags/Tag").Cast<XmlNode>().Select(x => x.InnerText).ToList();
                    target = db.Targets.Where(t => t.Name == targetname).FirstOrDefault();
                    if (target == null)
                    {
                        target = new Models.Target() { Name = targetname, XmlFilePath = Dowfile.FullName };
                        db.Targets.Add(target);
                    }
                    else
                    {
                        target.XmlFilePath = Dowfile.FullName;
                    }
                    target.Tags = new List<Tag>();
                    foreach (string s in tags)
                    {
                        target.Tags.Add(new Tag { TargetID = target.ID, tag = s });
                    }
                }
                catch(Exception e)
                {
                    return e.Message;
                }
                
                Dowfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.IncomingDatabase + "/" + targetname + ".dat"));
                uriAdress = string.Format("{0}/Target/Download?Identifier={1}&ID={2}&TargetName={3}&format={4}", Global.TargetHubAddress,
                    Global.Identifier, Global.HubID, targetname, "dat");
                try
                {
                    client.DownloadFile(uriAdress, Dowfile.FullName);
                    DownloadReplydat = "OK";
                    target.DatFilePath = Dowfile.FullName;
                }
                catch(Exception e)
                {
                    return e.Message;
                }

                db.SaveChanges();
                //Download Chat File
                Dowfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.ChatFolder + "/" + targetname + "_chat" + ".xml"));
                uriAdress = string.Format("{0}/Target/Download?Identifier={1}&ID={2}&TargetName={3}&format={4}", Global.TargetHubAddress,
                    Global.Identifier, Global.HubID, targetname, "chat");
                try
                {
                    client.DownloadFile(uriAdress, Dowfile.FullName);
                    target.ChatFilePath = Dowfile.FullName;
                }
                catch(Exception e)
                {
                    target.ChatFilePath = "";
                }
                
                db.SaveChanges();
            }

            return "Registration: " + RegisterationReply + "<br />" + "Download for xml: " + DownloadReplyxml + "<br />" +
                "<br />" + "Download for dat: " + DownloadReplydat + "<br />";
        }

        public static List<string> GetTargets(List<string> tags)
        {

            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress + "/Target/GetTargets?Identifier={0}&ID={1}",
                    Global.Identifier,Global.HubID);
                foreach (string s in tags )
                    uriAdress += "&Tags[]=" + s;
                var res = JsonConvert.DeserializeObject<List<string>>(Encoding.Default.GetString(client.DownloadData(uriAdress)));
                return res;
            }
        }

        internal static void SendMessage(string TargetName, string User, string SentMessage)
        {

            using (WebClient client = new WebClient())
            {
                string uriAdress = string.Format(Global.TargetHubAddress +
                    "/Target/ForwardMessage?Identifier={0}&ID={1}&TargetName={2}&UserName={3}&SentMessage={4}",
                    Global.Identifier, Global.HubID, TargetName, User, SentMessage);
                client.DownloadData(uriAdress);
            }
        }
    }
}