﻿/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using Space.NET.HTTP;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Space.NET.Core
{
    public static class CoreServer
    {
        internal static void Initialize(Settings settings)
        {
            var LogFilesPath = Path.Combine(settings.ServerRoot, Settings.LogsFolder);

            MyLog.Core     = new MyLog(LogFilesPath, "C# WebServer (Core)", "1.0.0");
            MyLog.Requests = new MyLog(LogFilesPath, "C# WebServer (Requests)", "1.0.0");
            MyLog.Requests.WriteInConsole = false;
            CrashLog.InitializeCrashLog();
            MyLog.Core.Write("Logs started!");

            HTTPServer.Initialize(settings);
            MyLog.Core.Write("HTTPServer initialized!");
        }

        internal static void Start()
        {
            MyLog.Core.Write("Starting HTTPServer!");
            HTTPServer.Start();
        }
    }
}
