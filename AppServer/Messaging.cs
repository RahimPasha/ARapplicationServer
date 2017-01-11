using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.IO;
using System.Text;
using ARApplicationServer.App_Code;

namespace ARApplicationServer
{
    //TODO: a Messaging object is handling communication of one specific target (as a chat room). it should have 
    // Chate file and file size and last message as members 
    public  class Messaging
    {
        
        #region Field Members
        private const int PollingSeconds = 60;
        private string FileAddress;
        private FileInfo Dfile;
        private double fileSize;
        private string lastMessage;
        private string targetName;
        private double clientFileSize;
        private string clientLastMessage;
        public bool FileSizeFlag;
        private readonly int pollingDuration;
        private string userName;
        public bool NewpollingReceived = false;
        //private bool changeTrigered = false;
        #endregion

        #region Constructors in C#
        public Messaging(string folderAdderss, string TargetName, string ClientLastMessage, double ClientFileSize,string User) 
            :this(folderAdderss, TargetName, ClientLastMessage, ClientFileSize, PollingSeconds,User)
        {
            //calling constructor in constructor            
        }
        public Messaging(string folderAdderss, string TargetName, string ClientLastMessage, double ClientFileSize, int PollingTime, string User)
        {
            this.FileAddress = HttpContext.Current.Server.MapPath(folderAdderss + "/" + TargetName + "_chat.xml");
            this.fileSize = 0;
            this.lastMessage = "";
            this.targetName = TargetName;
            this.clientFileSize = ClientFileSize;
            this.clientLastMessage = ClientLastMessage;
            this.pollingDuration = PollingTime;
            this.Dfile = new System.IO.FileInfo(FileAddress);
            this.FileSizeFlag = false;
            this.userName = User;
            this.prepareNewPolling(this);
        }

        //these two constructors are only for objects who wants to update the chat file.
        public Messaging(string folderAdderss, string TargetName, string User, string SentMessage, string sender)
        {
            FileAddress = HttpContext.Current.Server.MapPath(folderAdderss + "/" + TargetName + "_chat.xml");
            this.fileSize = 0;
            this.lastMessage = "";
            this.targetName = TargetName;
            this.clientFileSize = 0;
            this.clientLastMessage = SentMessage;
            this.pollingDuration = 60;
            this.Dfile = new System.IO.FileInfo(FileAddress);
            this.FileSizeFlag = false;
            this.UpdateChatFile(User, SentMessage);
            if(IsShared() && sender.ToLower() == "client")
            {
                THhandler.SendMessage(TargetName, User, SentMessage);
            }
            //this.userName = userName;
            //Messaging.Messages.Add(this);
        }

        public Messaging(string folderAdderss, string TargetName, string User, string SentMessage)
            : this(folderAdderss, TargetName, User, SentMessage,"NA")
        {            
        }

         #endregion

        private bool IsShared()
        {
            if (Dfile.Exists)
            {
                XmlDocument xDoc = new XmlDocument(); // reading XML documents
                xDoc.Load(Dfile.FullName);
                XmlNode shared = xDoc.SelectSingleNode("TargetChatFile/Shared");

                return shared != null ? (shared.InnerText == "True") ? true : false : false;
            }
            return false;
        }
        private void prepareNewPolling(Messaging messaging)
        {
            List<Messaging> temp = new List<Messaging>();
            foreach (Messaging m in Global.Messages)
            {
                if (m.Compare(this.userName, this.targetName))
                {
                    m.NewpollingReceived = true;
                    temp.Add(m);
                }
            }
            foreach(Messaging m in temp)
            {
                Global.Messages.Remove(m);
            }

            Global.Messages.Add(this);
        }
        public bool Compare(String User, string Target)
        {
            return (this.userName == User && this.targetName == Target) ? true : false;
        }
        private void UpdateChatFile(string User, string SentMessage)
        {
            if (!Dfile.Exists)
            {
                CreateChatFile(FileAddress, targetName);
            }
            if (Dfile.Exists)
            {
                XmlDocument xDoc = new XmlDocument(); // reading XML documents
                xDoc.Load(Dfile.FullName);
                XmlNode NumberOfMessages = xDoc.SelectSingleNode("/TargetChatFile/MessageNumber");
                fileSize = double.Parse(NumberOfMessages.InnerText);
                fileSize++;
                xDoc.SelectSingleNode("/TargetChatFile/MessageNumber").InnerText = fileSize.ToString();
                xDoc.SelectSingleNode("/TargetChatFile/LastMessage").InnerText = SentMessage;

                XmlElement newMessage = xDoc.CreateElement("Message");
                XmlElement newUser = xDoc.CreateElement("User");
                newUser.InnerText = User;
                XmlElement newBody = xDoc.CreateElement("Body");
                newBody.InnerText = SentMessage;
                newMessage.AppendChild(newUser);
                newMessage.AppendChild(newBody);
                xDoc.GetElementsByTagName("Messages")[0].InsertAfter(newMessage, xDoc.GetElementsByTagName("Messages")[0].LastChild);
                //xDoc.DocumentElement.AppendChild(newMessage);
                xDoc.Save(Dfile.FullName);
                NotifyListeners(targetName);
            }
        }

        private void NotifyListeners(string targetName)
        {
            foreach (Messaging m in Global.Messages)
            {
                if (m.targetName == this.targetName)
                    m.FileSizeFlag = true;
            }
        }

        public static string DecodeText(string Text)
        {
            return Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(Text));
        }
        public void ChatRequest()
        {
            if (!Dfile.Exists)
            {
                CreateChatFile(FileAddress, targetName);
            }
            else
            {
                XmlDocument xDoc = new XmlDocument(); // reading XML documents
                xDoc.Load(Dfile.FullName);
                XmlNode NumberOfMessages = xDoc.SelectSingleNode("/TargetChatFile/MessageNumber");
                fileSize = double.Parse(NumberOfMessages.InnerText);
                lastMessage = xDoc.SelectSingleNode("/TargetChatFile/LastMessage").InnerText;
            }

            if(fileSize == clientFileSize && lastMessage == clientLastMessage)
            {
                //start long polling and wait 60 secs then close connection
                //probably we need to implement an event which triggers if the chat file size changes
                LongPolling(this);
            }
            else
            {
                //Reply the chat file and close the connection
                Downloader.Download(Dfile); //connection will be automatically closed after the http request is processed.
            }
        }

        private  void LongPolling(Messaging ThisMessage)
        {

            //SubscribeToFileSizeEvent();
            int counter = pollingDuration * 2; //to check the flag every .5 second
            while (!FileSizeFlag && !(counter <= 10) && !NewpollingReceived)
            {
                System.Threading.Thread.Sleep(500);
                counter--;
            }
            if(FileSizeFlag)
            {
                FileSizeFlag = false;
                //File size has changed, so send the chat file
                Downloader.Download(Dfile); //connection will be automatically closed after the http request is processed.
            }
            //else
            //{

            //    //inform the client that no new message came for him/her and the connection got closed
            //    Downloader.Reply(new NameValueCollection() { { "Chat", "TimedOut" } });

            //}


        }

        private void SubscribeToFileSizeEvent()
        {
            var fsc = new FileSystemWatcher(Dfile.DirectoryName);
            //var fsc = new FileSystemWatcher(@"F:/Dropbox/UNBC/Thesis/visual studio 2013/ARapplicationServer/AppServer/Targets/Chat");

            fsc.Changed += fsc_Changed;
            fsc.Path = Dfile.DirectoryName;
            fsc.EnableRaisingEvents = true;
            //fsc.WaitForChanged(WatcherChangeTypes.All, 60000);
        }

        void fsc_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    var info = new FileInfo(e.FullPath);
                    XmlDocument xDoc = new XmlDocument(); // reading XML documents
                    if (e.Name == (this.targetName + "_chat.xml"))
                    {
                        xDoc.Load(e.FullPath);
                        XmlNode NumberOfMessages = xDoc.SelectSingleNode("/TargetChatFile/MessageNumber");
                        double newFileSize = double.Parse(NumberOfMessages.InnerText);
                        string newLastMessage = xDoc.SelectSingleNode("/TargetChatFile/LastMessage").InnerText;

                        if (newFileSize != this.fileSize)
                        {
                            this.FileSizeFlag = true;
                        }
                    }
                }
            }catch(Exception ex)
            {
               // Many change events are triggred for one change in the file. When a file is open and at the same time change trigger wants to accesss the file, it causes an exception.
            }
        }

        private  void CreateChatFile(string FileAddress, string TargetName)
        {
            string xmlString = "<TargetChatFile>\r\n";
            xmlString += "\t<TargetName>" + targetName + "</TargetName>\r\n";
            xmlString += "\t<MessageNumber>0</MessageNumber>\r\n";
            xmlString += "\t<LastMessage></LastMessage>\r\n";
            xmlString += "\t<Messages>\r\n";
            xmlString += "\t</Messages>";

            xmlString += "\r\n</TargetChatFile>";

            XmlDocument xDoc = new XmlDocument();
            xDoc.PreserveWhitespace = true;
            xDoc.LoadXml(xmlString);
            xDoc.Save(FileAddress);            
        }
    }
}