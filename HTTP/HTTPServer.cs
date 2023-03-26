/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using SpaceNET.API;
using SpaceNET.API.Utilities;
using SpaceNET.Core;
using SpaceNET.CSharp;
using SpaceNET.Utilities;
using HttpMultipartParser;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using Org.BouncyCastle.Utilities.Zlib;
using System.Threading;

namespace SpaceNET.HTTP
{
    internal static class HTTPServer
    {
        private static HttpListener HttpListener;
        //internal static Server APIServer;

        internal static List<Session> Sessions = new List<Session>();


        internal static void Initialize(Settings settings)
        {
            HttpListener = new HttpListener();

            Server.Settings = settings;
            //APIServer = new Server(settings);

            #region AddPrefixes
            var CorePrefix = "http://127.0.0.1:" + settings.DefaultPort + "/";
            try
            {
                HttpListener.Prefixes.Add(CorePrefix);
            }
            catch (Exception ex)
            {
                MyLog.Core.Write($"Cannot create Core prefix '{CorePrefix}'!", LogSeverity.Crash);
                MyLog.Core.Write(ex, LogSeverity.Crash);
                Program.Stop();
            }

            foreach (var prefix in settings.Prefixes)
            {
                if(prefix != "")
                    try
                    {
                        HttpListener.Prefixes.Add(prefix);
                    }
                    catch (Exception ex)
                    {
                        MyLog.Core.Write($"Cannot create prefix '{prefix}'!", LogSeverity.Warning);
                        MyLog.Core.Write(ex, LogSeverity.Warning);
                    }
            }

            #endregion AddPrefixes
            MyLog.Core.Write("Prefixes Added!");
        }


        internal static void Start()
        {
            HttpListener.Start();
            MyLog.Core.Write("HTTPListener started!");

            while (true)
            {
                var Context = HttpListener.GetContext();
                //ThreadPool.QueueUserWorkItem(delegate(object stateInfo) { HandleRequest(Context); });
                new Task(() => { HandleRequest(Context); }).Start();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static async void HandleRequest(HttpListenerContext ListenerContext)
        {
            HttpListenerRequest HTTPRequest = ListenerContext.Request;
            HttpListenerResponse HTTPResponse = ListenerContext.Response;
            
            #region Init_Request
            Request Request = GetRequest(ListenerContext);
            LogRequest(Request);
            MyLog.Core.Write("New Request : " + HTTPRequest.RawUrl, LogSeverity.NONE);

            #endregion Init_Request

            #region Init_Response
            Response Response = new Response(HTTPResponse);
            #endregion Init_Response

            #region ACCESS_PASSWORD
            if (Server.Settings.ACCESS_PASSWORD != "" && Path.GetFileName(Request.RealPath) != "favicon.ico")
            {
                var cookie = ListenerContext.Request.Cookies["ACCESS_PASSWORD"];

                if (cookie == null || cookie.Value != Server.Settings.ACCESS_PASSWORD)
                {
                    string response = "Access Denied [ACCESS_PASSWORD]<br><button onclick='document.cookie=`ACCESS_PASSWORD=` + prompt(`Password`) + `; expires=Fri, 31 Dec 9999 23:59:59 GMT`; location.reload();'>Set Password</button>";
                    Response.ContentType = "text/html; charset=utf-8";
                    Response.Write(response);
                    MyLog.Core.Write    ($"Request (from: {Request.ClientIPAddress}) blocked by [ACCESS_PASSWORD]", LogSeverity.Warning);
                    MyLog.Requests.Write($"Request (from: {Request.ClientIPAddress}) blocked by [ACCESS_PASSWORD]", LogSeverity.Warning);

                    goto RequestFinished;
                }
            }
            #endregion ACCESS_PASSWORD

            #region Init_Session
            Session Session = Request.Path.Contains("favicon") ? null : GetSession(ListenerContext);
            //if (Session != null)
            //    HTTPResponse.Cookies = Session.Cookies;
            #endregion Init_Session

            GET GET = null;
            POST POST = null;

            var Context = new RequestContext(Request, Session, GET, POST, null, Response);

            WebSocket WebSocket = null;
            if (HTTPRequest.IsWebSocketRequest)
            {
                WebSocket = new WebSocket(ListenerContext);
            }else
            {
                #region Parse_FormData


                (Dictionary<string, string>, Dictionary<string, FormFile>) ParsedFormData;

                try
                {
                    if (HTTPRequest.ContentType != null &&
                        HTTPRequest.ContentType.StartsWith("multipart/form-data"))
                    {
                        var FormDataParser = MultipartFormDataParser.Parse(HTTPRequest.InputStream, HTTPRequest.ContentType.Split("; boundary=")[1], HTTPRequest.ContentEncoding);
                        ParsedFormData = ParseHTMLData(FormDataParser);

                    }
                    else
                    {

                        //StreamReader stream = new StreamReader(HTTPRequest.InputStream, HTTPRequest.ContentEncoding);
                        //string Data = stream.ReadToEnd();
                        string Data = HTTPRequest.Url.Query;
                        Data = Data.StartsWith('?') ? Data.Substring(1) : Data;
                        ParsedFormData = ParseHTMLData(Data);
                    }
                }
                catch (Exception ex)
                {
                    CompiledScripts.ErrorScripts["400"].Execute(Context);

                    MyLog.Core.Write("Error parsing Form-Data", LogSeverity.Warning);
                    MyLog.Core.Write(ex, LogSeverity.Warning);

                    goto RequestFinished;
                }

                #endregion Parse_FormData

                #region Init_GET_POST

                if (Request.Method == "GET")
                    GET = new GET(ParsedFormData.Item1, ParsedFormData.Item2);
                else if (Request.Method == "POST")
                    POST = new POST(ParsedFormData.Item1, ParsedFormData.Item2);

                #endregion Init_GET_POST
            }
            Context = new RequestContext(Request, Session, GET, POST, WebSocket, Response);
            #region HandleFile
            bool IsFile = File.Exists(Request.RealPath);
            bool IsFolder = Directory.Exists(Request.RealPath);
            string Extension = Path.GetExtension(Request.RealPath);

            // if Path is Script
            if (Extension == ".cshtml")
            {
                Handle_Script(Context);
                if(Request.IsWebSocketRequest)
                    goto DontCloseRequest;
            }
            else
            {
                // if Path is normal File
                if (IsFile)
                {
                    Response.Headers.Add("Content-Type: " + MIME.GetMimeType(Extension));
                    Response.Write(File.ReadAllBytes(Request.RealPath));
                }
                // if Path is Folder
                else if (IsFolder)
                {
                    //if User was already moved to Folder, then respond with the index.html
                    if (Request.URL.EndsWith('/'))
                    {
               
                        Request.Path += "index.cshtml";
                        Request.URL += "index.cshtml";
                        if (File.Exists(Request.RealPath))
                            Handle_Script(Context);
                        else
                            CompiledScripts.ErrorScripts["404"].Execute(Context);
                    }
                    //if User requests a Folder, then redirect
                    else
                    {
                        try
                        {
                            var path = Path.GetDirectoryName(Request.Path);
                            if (path == null)
                                throw new Exception();
                            path = path.Replace("\\", "/") + "/";
                            Response.Redirect = path;
                            goto RequestFinished;
                        }
                        catch (Exception ex)
                        {
                            CompiledScripts.ErrorScripts["500"].Execute(Context);
                        }

                    }

                }
                //If user requested a file that does not exist
                else
                {
                    CompiledScripts.ErrorScripts["404"].Execute(Context);
                }
            }
        #endregion HandleFile


        RequestFinished:
            if (Response.Redirect != null)
            {
                HTTPResponse.Redirect(Response.Redirect);
                HTTPResponse.Close();
            }
            else
            {
                // No Code optimization
                while (Response.CurrentLength < Response.ContentLength)
                { }

                try
                {
                    HTTPResponse.Close();
                }
                catch (Exception)
                {

                }
            }
        DontCloseRequest:
            ;
        }

        private static Session GetSession(HttpListenerContext Context)
        {
            if (Context.Request.Cookies["SessionID"] == null ||
                Sessions.Find(x => x.SessionID == Context.Request.Cookies["SessionID"].Value) == null)
            {
                string Unique = Context.Request.RemoteEndPoint.ToString() + DateTime.UtcNow.Ticks.ToString();
                string SessionID = Hashing.GetHash(Hashing.SHA256, Unique);

                Context.Response.SetCookie(new Cookie("SessionID", SessionID, "/"));

                var Session = new Session(SessionID, Context.Request.Cookies);
                Sessions.Add(Session);
                return Session;
            }
            else
            {
                return Sessions.Find(x => x.SessionID == Context.Request.Cookies["SessionID"].Value);
            }
        }



        private static Request GetRequest(HttpListenerContext Context)
        {
            var request = new Request(Context.Request.HttpMethod,
                               Context.Request.Url.ToString(),
                               HttpUtility.UrlDecode(Context.Request.Url.AbsolutePath),
                               Context.Request.RemoteEndPoint.ToString(),
                               Context.Request.UserAgent,
                               Guid.NewGuid().ToString(),
                               Context.Request.IsWebSocketRequest);

            if (request.Path != "/")
                request.Path += (Directory.Exists(request.RealPath) ? "/" : "");

            if (request.Path.StartsWith("/ErrorDocs"))
            {
                request.Path = "/.." + request.Path;
            }

            return request;
        }

        private static void LogRequest(Request Request)
        {
            if (Request.Path.Contains("favicon") == false)
                MyLog.Requests.Write($"Incoming Request:\n" +
                                     $"Method:     {Request.Method}\n" +
                                     $"URL:        {Request.URL}\n" +
                                     $"Path:       {Request.Path}\n" +
                                     $"IP-Address: {Request.ClientIPAddress}\n" +
                                     $"User Agent: {Request.UserAgent}\n" +
                                     $"TraceID:    {Request.TraceID}\n");
        }

        private static void Handle_Script(RequestContext Context)
        {

            //If Script Exists in FileSystem
            if (File.Exists(Context.Request.RealPath))
            {
                //if HTMLScript is already compiled before
                if (CompiledScripts.Scripts.ContainsKey(Context.Request.Path))
                {
                    HTMLScript Script = CompiledScripts.Scripts[Context.Request.Path];

                    //If File of Script not edited
                    if (CompiledScripts.Compare(Context.Request.RealPath, Script) == true)
                    {
                        Script.Execute(Context);
                    }
                    else
                    {
                        Script.Compile();
                        Script.Execute(Context);
                    }
                }
                else
                {
                    //Compile the Script
                    HTMLScript Script = new HTMLScript(Context.Request.RealPath);
                    Script.Compile();
                    lock (CompiledScripts.DictionaryLOCK)
                    {
                        if(CompiledScripts.Scripts.ContainsKey(Context.Request.Path) == false)
                            CompiledScripts.Scripts.Add(Context.Request.Path, Script);
                    }

                    //Execute the Script
                    Script.Execute(Context);
                }
            }
            else
            {
                //Return 404
                CompiledScripts.ErrorScripts["404"].Execute(Context);
            }
        }

        private static (Dictionary<string, string>, Dictionary<string, FormFile>) ParseHTMLData(MultipartFormDataParser FormData)
        {
            Dictionary<string, string> Arguments = new Dictionary<string, string>();
            Dictionary<string, FormFile> Files = new Dictionary<string, FormFile>();

            foreach (var parameter in FormData.Parameters)
            {
                Arguments.Add(parameter.Name, parameter.Data);
            }

            foreach (var file in FormData.Files)
            {
                Files.Add(file.Name, new FormFile(file.FileName, file.Data));
            }

            return (Arguments, Files);
        }

        private static (Dictionary<string, string>, Dictionary<string, FormFile>) ParseHTMLData(string HtmlEncoded)
        {
            Dictionary<string, string> Arguments = new Dictionary<string, string>();
            Dictionary<string, FormFile> Files = new Dictionary<string, FormFile>();

            if (HtmlEncoded.Length > 0)
                foreach (var argument in HtmlEncoded.Split('&'))
                {
                    string key = argument.Split('=')[0];
                    string value = HttpUtility.UrlDecode(argument.Split('=')[1]);
                    Arguments.Add(key, value);
                }

            return (Arguments, Files);
        }
    }
}
