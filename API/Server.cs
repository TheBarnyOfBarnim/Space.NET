/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2022-2024 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using SpaceNET.API.Utilities;
using SpaceNET.Core;
using System.IO;

namespace SpaceNET.API
{
    public static class Server
    {
        public static Settings Settings { get; internal set; }

        public static string ServerRoot => Settings.ServerRoot;
        public static string LogFilesFolder => Path.Combine(Settings.ServerRoot, Settings.LogsFolder);
        public static string ErrorDocsFolder => Path.Combine(Settings.ServerRoot, Settings.ErrorDocsFolder);
        public static string DocumentRoot => Path.Combine(Settings.ServerRoot, Settings.DocumentRootFolder);
        public static string StaticFolder => Path.Combine(Settings.ServerRoot, Settings.StaticFolder);


        public static IndexerObject Globals = new IndexerObject();


        public static new string ToString()
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
