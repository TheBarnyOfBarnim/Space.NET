/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using Space.NET.CSharp;
using System;
using System.IO;
using Utilities;

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