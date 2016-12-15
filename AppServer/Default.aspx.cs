using System;
using System.Web;
using System.Collections.Specialized;
using System.Xml;
using ARApplicationServer.App_Code;
using System.Configuration;

namespace ARApplicationServer
{
    public partial class Default : System.Web.UI.Page
    {
        string fileAddress = "";
        string Parameter1;
        string fileName;
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpRequest q = Request;
            NameValueCollection MyQueryString = q.QueryString;
            Global.TargetHubAddress = ConfigurationManager.AppSettings["HubAddress"].ToString();
            Global.serverConfigFile = "Config" + "/" + "Config.xml";
            Global.Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(Global.serverConfigFile));
            Global.xDoc.Load(Global.Dfile.FullName);
            Global.ServerName = Global.xDoc.SelectSingleNode("Server/Name").InnerText;
            Global.ServerID = Global.xDoc.SelectSingleNode("Server/ID").InnerText;
            Global.Registered = Global.xDoc.SelectSingleNode("Server/Registered").InnerText;
            Global.Identifier = Global.xDoc.SelectSingleNode("Server/Identifier").InnerText;
            Global.TargetsFolder = Global.xDoc.SelectSingleNode("Server/Targets/TargetsFolder").InnerText;
            Global.incomingDatabase = Global.xDoc.SelectSingleNode("Server/Targets/Incoming").InnerText;
            Global.outgoingDatabase = Global.xDoc.SelectSingleNode("Server/Targets/Outgoing").InnerText;
            Global.ChatFolder = Global.xDoc.SelectSingleNode("Server/Targets/ChatFile").InnerText;
            Global.Tags = Global.xDoc.GetElementsByTagName("Tags");
            

            if (MyQueryString.HasKeys()) // On each target detection an empty request comes to the server from the client
            //may be in the future
            {
                Parameter1 = MyQueryString.GetKey(0);
                if (Parameter1.ToLower() == "file")
                {
                    //fileNmae = MyQueryString.Get(0);
                    fileName = "Database" + MyQueryString.Get(0).Substring(MyQueryString.Get(0).LastIndexOf('.'));
                    fileAddress = Global.TargetsFolder;
                    Downloader.Download(fileName, fileAddress);
                }
                
                else if (Parameter1.ToLower() == "shared")
                {
                    //fileNmae = MyQueryString.Get(0);
                    fileAddress = Global.incomingDatabase;
                    fileName = "shared" + MyQueryString.Get(0).Substring(MyQueryString.Get(0).LastIndexOf('.'));
                    Downloader.Download(fileName, fileAddress);
                }
                else if (Parameter1.ToLower() == "register")
                {
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.Write(THhandler.Register());
                    HttpContext.Current.Response.Flush();
                    //HttpContext.Current.Response.Close();
                }
                else if (Parameter1.ToLower() == "upload")
                {
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.Write(THhandler.Register());
                    HttpContext.Current.Response.Write("\n" + THhandler.Upload());
                    HttpContext.Current.Response.Flush();
                    //HttpContext.Current.Response.Close();
                }
                else if (Parameter1.ToLower() == "chat")
                {
                    //fileNmae = MyQueryString.Get(0);
                    fileAddress = Global.ChatFolder;
                    string targetName = MyQueryString.Get(0);
                    if (MyQueryString.GetKey(1) == "lastMessage")
                    {
                        string temp = MyQueryString.Get("lastMessage");
                        string lastMessage = Messaging.DecodeText(temp);
                        
                        double numberOfMessages = double.Parse(MyQueryString.Get("FileSize"));

                        temp = MyQueryString.Get("User");
                        string User = Messaging.DecodeText(temp);
                        Messaging ChatHadnler = new Messaging(fileAddress, targetName, lastMessage, numberOfMessages,User);
                        ChatHadnler.ChatRequest();
                    }
                    else if (MyQueryString.GetKey(1) == "SentMessage")
                    {
                        string temp = MyQueryString.Get("SentMessage");
                        string SentMessage = Messaging.DecodeText(temp);
                        temp = MyQueryString.Get("User");
                        string User = Messaging.DecodeText(temp);
                        Messaging ChatHadnler = new Messaging(fileAddress, targetName, User, SentMessage);
                        HttpContext.Current.Response.ClearContent();
                        HttpContext.Current.Response.Flush();
                    }
                }
            }
        }             
    }
}