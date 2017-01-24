using System;
using System.Web;
using System.Collections.Specialized;
using ARApplicationServer.App_Code;
using System.Linq;
using Newtonsoft.Json;
using System.Web.Services;
using ARApplicationServer.Models;

namespace ARApplicationServer
{
    public partial class Default : System.Web.UI.Page
    {
        DAL db = new DAL();
        string fileAddress = "";
        string Parameter1;
        string fileName;
        [WebMethod]
        public static bool Ping(string value)
        {
            return true;
        }
        
        [WebMethod]
        public static bool CheckPass(string username, string password)
        {
            using (DAL db = new DAL())
            {
               return db.Users.Where(u => u.Name == username && u.Password == password).Count() == 0 ? false : true;
            }
        }

        [WebMethod]
        public static bool AddTarget(string username, string password, string TargetName)
        {
            try
            {
                using (DAL db = new DAL())
                {
                    if (db.Users.Where(u => u.Name == username && u.Password == password).Count() != 0)
                    {
                        Target newT = new Target()
                        {
                            Name = TargetName,
                            DatFilePath = HttpContext.Current.Server.MapPath("~") + Global.MyServer.TargetsFolder +
                            "\\" + TargetName + ".dat",
                            XmlFilePath = HttpContext.Current.Server.MapPath("~") + Global.MyServer.TargetsFolder +
                            "\\" + TargetName + ".xml",
                        };
                        if (db.Targets.Where(t => t.Name == TargetName).Count() == 0)
                        {
                            db.Targets.Add(newT);
                            db.SaveChanges();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public static string AddRootFolder(string username, string password)
        {
            try
            {
                using (DAL db = new DAL())
                {
                    db.Servers.Where(s => s.ID == Global.ActiveServerInfo).FirstOrDefault().RootFolder =
                        HttpContext.Current.Server.MapPath("~");
                    db.SaveChanges();
                    return "Root Folder Added";
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpRequest q = Request;
            NameValueCollection MyQueryString = q.QueryString;
            if (MyQueryString.HasKeys())
            {
                Parameter1 = MyQueryString.GetKey(0);
                if (Parameter1.ToLower() == "file")
                {
                    try
                    {
                        fileName = MyQueryString.Get(0);
                        int index = fileName.LastIndexOf('.');
                        string targetName = fileName.Substring(0, index);
                        Target target;
                        target = db.Targets.Where(t => t.Name == targetName).FirstOrDefault();
                        if (target == null || target.DatFilePath == null || target.XmlFilePath == null)
                        {
                            THhandler.Download(targetName);
                            target = db.Targets.Where(t => t.Name == targetName).FirstOrDefault();
                        }
                        if (target != null && target.DatFilePath != null && target.XmlFilePath != null) // be careful not to use else if
                        {
                            string ext = fileName.Substring(index + 1);
                            fileAddress = (ext == "dat") ?
                                target.DatFilePath : target.XmlFilePath;
                            Downloader.Download(fileAddress);
                        }
                    }
                    catch (Exception exc)
                    {
                        HttpContext.Current.Response.ClearContent();
                        HttpContext.Current.Response.Write(exc.Message);
                        HttpContext.Current.Response.Flush();
                    }
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
                    HttpContext.Current.Response.Write("\n" + THhandler.Upload());
                    HttpContext.Current.Response.Flush();
                    //HttpContext.Current.Response.Close();
                }
                else if (Parameter1.ToLower() == "download")
                {
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.Write("\n" + THhandler.Download(MyQueryString.Get(0)));
                    HttpContext.Current.Response.Flush();
                    //HttpContext.Current.Response.Close();
                }

                else if (Parameter1.ToLower() == "get")
                {
                    HttpContext.Current.Response.ClearContent();
                    try
                    {
                        if (MyQueryString.GetKey(1).ToLower().Contains("tag"))
                        {
                            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(
                                THhandler.GetTargets(MyQueryString.Get(1).Split(',').ToList())));
                        }
                    }
                    catch (Exception)
                    {
                        HttpContext.Current.Response.Write("Bad Format or an internal error");
                    }
                    HttpContext.Current.Response.Flush();
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
                        Messaging ChatHadnler = new Messaging(fileAddress, targetName, lastMessage, numberOfMessages, User);
                        ChatHadnler.ChatRequest();
                    }
                    else if (MyQueryString.GetKey(1) == "SentMessage")
                    {
                        string SentMessage = Messaging.DecodeText(MyQueryString.Get("SentMessage"));
                        string User = Messaging.DecodeText(MyQueryString.Get("User"));
                        string temp = MyQueryString.Get("Sender");
                        string Sender = Messaging.DecodeText(temp);
                        Messaging ChatHadnler = new Messaging(fileAddress, targetName, User, SentMessage, Sender);
                        HttpContext.Current.Response.ClearContent();
                        HttpContext.Current.Response.Flush();
                    }

                }
            }
        }             
    }
}