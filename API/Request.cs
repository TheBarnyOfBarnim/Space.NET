/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Space.NET.Core;
using Space.NET.HTTP;

namespace Space.NET.API
{
    public class Request
    {
        public string Method { get; private set; }
        public string URL { get; internal set; }
        public string URLFolder => (System.IO.Path.GetDirectoryName(URL) + "").Replace("\\", "/");
        public string Path { get; internal set; }
        public string WebFolder => (System.IO.Path.GetDirectoryName(Path) + "").Replace("\\", "/");
        public string RealPath => System.IO.Path.GetFullPath(System.IO.Path.Combine(HTTPServer.APIServer.DocumentRoot + Path));
        public string RealPathFolder => System.IO.Path.GetDirectoryName(RealPath);
        public string ClientIPAddress { get; private set; }
        public string UserAgent { get; private set; }
        public string TraceID { get; private set; }

        internal Request(string method, string url, string path, string clientIPAdress, string userAgent, string connectionID)
        {
            Method = method;
            URL = url;
            Path = path;
            ClientIPAddress = clientIPAdress;
            UserAgent = userAgent;
            TraceID = connectionID;
        }

        public override string ToString()
        {
            string output = "";

            output += "Method: " + Method + "\n";
            output += "URL: " + URL + "\n";
            output += "Path: " + Path + "\n";
            output += "RealPath: " + RealPath + "\n";
            output += "ClientIPAddress: " + ClientIPAddress + "\n";
            output += "UserAgent: " + UserAgent + "\n";
            output += "TraceID: " + TraceID + "\n";

            return output;
        }
    }
}
