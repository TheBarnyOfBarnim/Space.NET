/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using Space.NET.Utilities;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Space.NET.Core
{
    public class Settings
    {
        internal const string ConfigFileName = "Config.json";
        internal const string LogsFolder = "Logs";
        internal const string ErrorDocsFolder = "ErrorDocs";
        internal const string DocumentRootFolder = "DocumentRoot";
        

        internal bool FileExists = true;

        [JsonPropertyAttribute]
        public string ServerRoot { get; set; }
        [JsonPropertyAttribute]
        public string[] Prefixes { get; set; }

        [JsonPropertyAttribute]
        public int DefaultPort { get; set; }


        internal static Settings GetSettingsFromConfig()
        {
            if (File.Exists(ConfigFileName) == false)
                return new Settings() { FileExists = false };

            try
            {
                string FileContent = File.ReadAllText(ConfigFileName);
                var settings = JsonConvert.DeserializeObject<Settings>(FileContent);

                settings.Verify();
                return settings;
            }
            catch (Exception ex) { Console.WriteLine("[Error] Settings.GetSettingsFromConfig();\n" + ex.ToString()); Program.Stop(); throw; }
        }

        internal static Settings Setup()
        {
            Console.Clear();
            ConsoleQuestions.WriteTitlebar("C# WebServer - Setup");
            ConsoleQuestions.WriteQuestionHead("Looks like the WebServer was not set up yet!");

            string ServerRootFolderPath;
            string[] Prefixes;
            int defaultPort;

            #region AskUser
            ServerRootFolderPath = ConsoleQuestions.AskPathInput("Where should the WebServer Root Folder be?");
            int.TryParse(ConsoleQuestions.AskStringInput("On What Default Port should the WebServer listen? [11829]"), out var port);
            if (port == 0)
                port = 11829;
            defaultPort = port;

            var PrefixesAnswer = ConsoleQuestions.AskStringInput("On what Ip-Addresses/Domain names should the WebServer listen? (enter a comma seperated list!)");
            if (PrefixesAnswer == "")
                Prefixes = new string[0];
            else
            {
                PrefixesAnswer = PrefixesAnswer.Replace(" ", "");
                PrefixesAnswer = PrefixesAnswer.Trim();
                Prefixes = PrefixesAnswer.Split(",");
            }

            #endregion AskUser

            var settings = new Settings() {DefaultPort = defaultPort, ServerRoot= ServerRootFolderPath, Prefixes = Prefixes};
            settings.Verify();

            ConsoleQuestions.WriteQuestionHead("All settings are set! Saving file to 'Config.json' in the current Folder.");

            var JSONString = JsonConvert.SerializeObject(settings);
            File.WriteAllText(ConfigFileName, JSONString);
            ConsoleQuestions.WriteQuestionHead("Done saving 'Config.json'!");

            return settings;
        }

        internal void Verify()
        {
            try
            {
                Directory.CreateDirectory(ServerRoot);
                Directory.CreateDirectory(Path.Combine(ServerRoot, LogsFolder));
                Directory.CreateDirectory(Path.Combine(ServerRoot, ErrorDocsFolder));
                Directory.CreateDirectory(Path.Combine(ServerRoot, DocumentRootFolder));
            }
            catch (Exception ex) { Console.WriteLine("[Error] Settings.Verify();\n" + ex.ToString()); Program.Stop(); }
        }
    }
}
