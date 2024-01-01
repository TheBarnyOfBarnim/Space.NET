/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using Newtonsoft.Json;
using SpaceNET.Utilities;
using System;
using System.IO;

namespace SpaceNET.Core
{
    public class Settings
    {
        internal const string ConfigFileName = "Config.json";
        internal const string LogsFolder = "Logs";
        internal const string ErrorDocsFolder = "ErrorDocs";
        internal const string DocumentRootFolder = "DocumentRoot";
        internal const string StaticFolder = "Static";


        internal bool FileExists = true;

        [JsonPropertyAttribute]
        public string ServerRoot { get; set; }
        [JsonPropertyAttribute]
        public string[] Prefixes { get; set; }

        [JsonPropertyAttribute]
        public int DefaultPort { get; set; }

        [JsonPropertyAttribute]
        public string ACCESS_PASSWORD { get; set; }


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
            string accessPassword;

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

            accessPassword = ConsoleQuestions.AskStringInput("Should the WebServer have a password, \nso clients can only request data if they have the correct \nCookie 'ACCESS_PASSWORD'? []");

            #endregion AskUser

            var settings = new Settings() { DefaultPort = defaultPort, ServerRoot = ServerRootFolderPath, Prefixes = Prefixes, ACCESS_PASSWORD = accessPassword };
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
                Directory.CreateDirectory(Path.Combine(ServerRoot, StaticFolder));

                if (File.Exists(Path.Combine(ServerRoot, ErrorDocsFolder, "400.cshtml")) == false)
                    File.WriteAllText(Path.Combine(ServerRoot, ErrorDocsFolder, "400.cshtml"), @"<cs>Response.StatusCode = 400;</cs>
                                                                                     <html>
                                                                                     	<head>
                                                                                     		<title>400: Bad Request</title>
                                                                                     	</head>
                                                                                     	<body style=""max-width:50%"">
                                                                                        		<div style=""padding: 10px; border: solid; border-width: 3px;"">
                                                                                     			<h1 style=""color:red;"">Bad Request</h1>
                                                                                         		<p>The Server received invalid Form-Data from the User.</p>
                                                                                         		<p>Please contact the server administrator and inform them of the time the error occured, and anything you might have done before that may have caused the error.</p>
                                                                                         		<p>&nbsp;</p>
                                                                                         		<p>More information about this error may be available in the server's Core Log.</p>
                                                                                     		</div>   
                                                                                     	</body>
                                                                                     </html>");

                if (File.Exists(Path.Combine(ServerRoot, ErrorDocsFolder, "404.cshtml")) == false)
                    File.WriteAllText(Path.Combine(ServerRoot, ErrorDocsFolder, "404.cshtml"), @"<cs>Response.StatusCode = 404;</cs>
                                                                                     <html>
                                                                                     	<head>
                                                                                     		<title>404: Not Found</title>
                                                                                     	</head>
                                                                                     	<body style=""max-width:50%"">
                                                                                        		<div style=""padding: 10px; border: solid; border-width: 3px;"">
                                                                                     			<h1 style=""color:red;"">Not Found</h1>
                                                                                         		<p>The resource you are looking for does not exist on the server.</p>
                                                                                     		</div>   
                                                                                     	</body>
                                                                                     </html>");

                if (File.Exists(Path.Combine(ServerRoot, ErrorDocsFolder, "500.cshtml")) == false)
                    File.WriteAllText(Path.Combine(ServerRoot, ErrorDocsFolder, "500.cshtml"), @"<cs>Response.StatusCode = 500;</cs>
                                                                                     <html>
                                                                                     	<head>
                                                                                     		<title>500: Internal Server Error</title>
                                                                                     	</head>
                                                                                     	<body style=""max-width:50%"">
                                                                                        		<div style=""padding: 10px; border: solid; border-width: 3px;"">
                                                                                     			<h1 style=""color:red;"">Internal Server Error</h1>
                                                                                         		<p>The Server encountered an internal Error.</p>
                                                                                         		<p>Please contact the server administrator and inform them of the time the error occured, and anything you might have done before that may have caused the error.</p>
                                                                                         		<p>&nbsp;</p>
                                                                                         		<p>More information about this error may be available in the server's Core Log.</p>
                                                                                     		</div>   
                                                                                     	</body>
                                                                                     </html>");
            }
            catch (Exception ex) { Console.WriteLine("[Error] Settings.Verify();\n" + ex.ToString()); Program.Stop(); }
        }
    }
}
