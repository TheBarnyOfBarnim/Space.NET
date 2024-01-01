/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2022-2024 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using SpaceNET.Utilities;
using System;

namespace SpaceNET.Core
{
    internal class Program
    {
        public static string[] Arguments = new string[0];

        static void Main(string[] args)
        {
            Arguments = args;
            Console.WriteLine("Arguments:");
            foreach (var arg in Arguments)
            {
                Console.WriteLine("\t" + arg);
            }


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