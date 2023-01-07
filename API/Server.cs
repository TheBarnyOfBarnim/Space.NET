/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Space.NET.API.Utilities;
using Space.NET.Core;

namespace Space.NET.API
{
    public class Server
    {
        public readonly Settings Settings;
        public Server(Settings settings)
        {
            Settings = settings;
        }

        public string ServerRoot => Settings.ServerRoot;
        public string LogFilesFolder => Path.Combine(Settings.ServerRoot, Settings.LogsFolder);
        public string ErrorDocsFolder => Path.Combine(Settings.ServerRoot, Settings.ErrorDocsFolder);
        public string DocumentRoot => Path.Combine(Settings.ServerRoot, Settings.DocumentRootFolder);


        public IndexerObject Globals = new IndexerObject();


        public override string ToString()
        {
            string output = "";

            output += "ServerRoot: " + ServerRoot + "\n";
            output += "LogFilesFolder: " + LogFilesFolder + "\n";
            output += "ErrorDocsFolder: " + ErrorDocsFolder + "\n";
            output += "DocumentRoot: " + DocumentRoot + "\n";
            return output;
        }
    }
}
