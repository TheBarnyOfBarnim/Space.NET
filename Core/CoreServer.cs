﻿/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using SpaceNET.HTTP;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceNET.Utilities;
using SpaceNET.Scripting.Static;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SpaceNET.CSharp;
using System.Reflection;

namespace SpaceNET.Core
{
    internal static class CoreServer
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


            CompiledScripts.InitializeStatic();

            CompiledScripts.ExecuteStartScript();

            CompiledScripts.Initialize();
        }

        internal static void Start()
        {
            MyLog.Core.Write("Starting HTTPServer!");
            HTTPServer.Start();
        }
    }
}
