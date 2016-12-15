using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

namespace ARApplicationServer
{

    /// <summary>
    /// Summary description for Downloader
    /// </summary>
    public static class Downloader
    {
        public static void Download(string sFileName)
        {
            Downloader.Download(sFileName, "");
        }
        public static void Download(string sFileName, string sFilePath)
        {
            System.IO.FileInfo Download_File = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(sFilePath + "/" + sFileName));
            Download(Download_File);
        }
        //in case I need to reply the request only some parameters (and a message).
        public static void Reply(NameValueCollection MyHeaders)
        {
            Reply(MyHeaders, "");
        }
        public static void Reply(NameValueCollection MyHeaders, String Message)
        {
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.AddHeader("Content-Type", "text/plain");
            foreach (string s in MyHeaders.AllKeys)
            {
                HttpContext.Current.Response.Headers.Add(s, MyHeaders[s]);
            }
            if (HttpContext.Current.Response.IsClientConnected)
            {
                HttpContext.Current.Response.Write(Message);
                HttpContext.Current.Response.Flush();
            }
        }
        public static void Download(System.IO.FileInfo Dfile)
        {
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentType = "APPLICATION/OCTET-STREAM";
            String Header = "Attachment; Filename=" + Dfile.Name;
            HttpContext.Current.Response.AppendHeader("Content-Disposition", Header);
            HttpContext.Current.Response.AppendHeader("Content-Length", Dfile.Length.ToString());
            HttpContext.Current.Response.WriteFile(Dfile.FullName);
            try
            {

                if (Dfile.Exists)
                {
                    HttpContext.Current.Response.Headers.Add("status", "OK");
                    HttpContext.Current.Response.Headers.Add("File", "Downlaod");
                    HttpContext.Current.Response.WriteFile(Dfile.FullName);
                }
                else
                {
                    HttpContext.Current.Response.Headers.Add("file", "Not found");
                }
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Headers.Add("Error", e.Message);
            }

            HttpContext.Current.Response.Flush();
        }
        public static void upload(string sFileName, string sFilePath, HttpRequest Request)
        {
            System.IO.FileInfo Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(sFilePath + "/" + sFileName));
            HttpPostedFile file;

            foreach (string f in Request.Files.AllKeys)
            {
                file = Request.Files[f];
                file.SaveAs(Dfile.FullName);
            }	
        }

    }
}