/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using Space.NET.CSharp;
using Space.NET.Utilities;
using System;
using System.IO;

namespace Space.NET.Core
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var settings = Settings.GetSettingsFromConfig();


            if (settings.FileExists == false)
            {
                settings = Settings.Setup();

                CoreServer.Initialize(settings);
                MyLog.Core.Write("CoreServer initialized!");
                CoreServer.Start();
            }
            else
            {
                CoreServer.Initialize(settings);
                MyLog.Core.Write("CoreServer initialized!");
                CoreServer.Start();
            }
        }

        internal static void Stop(int Code = 0)
        {
            Environment.Exit(Code);
        }
    }
}