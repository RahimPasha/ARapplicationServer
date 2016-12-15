using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml;
using System.Text;

namespace ARApplicationServer
{
    public partial class Default : System.Web.UI.Page
    {
        string fileAddress = "";
        string Parameter1;
        string fileName;
        string ServerName;
        string incomingDatabase;
        string outgoingDatabase;
        string TargetsFolder;
        string ChatFolder;
        string ServerID;
        string serverConfigFile;
        System.IO.FileInfo Dfile;
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpRequest q = Request;
            NameValueCollection MyQueryString = q.QueryString;
            serverConfigFile = "Config" + "/" + "Config.xml";
            Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(serverConfigFile));
            //string fileName = "ServerA\\Config.xml";
            
            XmlDocument xDoc = new XmlDocument(); // reading XML documents
            xDoc.Load(Dfile.FullName);
            ServerName = xDoc.SelectSingleNode("Server/Name").InnerText;
            ServerID = xDoc.SelectSingleNode("Server/ID").InnerText;
            TargetsFolder = xDoc.SelectSingleNode("Server/Targets/TargetsFolder").InnerText;
            incomingDatabase = xDoc.SelectSingleNode("Server/Targets/Incoming").InnerText;
            outgoingDatabase = xDoc.SelectSingleNode("Server/Targets/Outgoing").InnerText;
            ChatFolder = xDoc.SelectSingleNode("Server/Targets/ChatFile").InnerText;
            //HttpContext.Current.Response.Write(Dfile.FullName+"--"+outgoingDatabase);

            if (MyQueryString.HasKeys()) // On each target detection an empty request comes to the server from the client
            //may be in the future
            {
                Parameter1 = MyQueryString.GetKey(0);
                if (Parameter1.ToLower() == "file")
                {
                    //fileNmae = MyQueryString.Get(0);
                    fileName = "Database" + MyQueryString.Get(0).Substring(MyQueryString.Get(0).LastIndexOf('.'));
                    fileAddress = TargetsFolder;
                    Downloader.Download(fileName, fileAddress);
                }
                
                if (Parameter1.ToLower() == "shared")
                {
                    //fileNmae = MyQueryString.Get(0);
                    fileAddress = incomingDatabase;
                    fileName = "shared" + MyQueryString.Get(0).Substring(MyQueryString.Get(0).LastIndexOf('.'));
                    Downloader.Download(fileName, fileAddress);
                }

                // TODO: Server should not register itself. It is process initiated by the user by calling an API of the TH. ServerA even doesn't
                // know the address of Target Hub.
                if (Parameter1.ToLower() == "register")
                {
                    //HttpContext.Current.Response.ClearHeaders();
                    //HttpContext.Current.Response.ContentType = "text/plain";
                    //String Header = "Attachment; Filename=";
                    //HttpContext.Current.Response.AppendHeader("Content-Disposition", Header);
                    //System.IO.FileInfo Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(sFilePath + "/" + sFileName));

                    HttpContext.Current.Response.ClearContent();
                    //HttpContext.Current.Response.WriteFile(Dfile.FullName);
                    //HttpContext.Current.Response.Write("Hellooo1!");
                    //HttpContext.Current.Response.Write(Dfile.FullName);

                    HttpContext.Current.Response.Flush();
                    //HttpContext.Current.Response.Close();
                    THhandler.Register(serverConfigFile);
                }
                if (Parameter1.ToLower() == "chat")
                {
                    //fileNmae = MyQueryString.Get(0);
                    fileAddress = ChatFolder;
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