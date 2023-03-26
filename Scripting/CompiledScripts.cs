/*!
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
using SpaceNET.Core;
using SpaceNET.API.Utilities;
using SpaceNET.API;

namespace SpaceNET.CSharp
{
    internal static class CompiledScripts
    {
        internal static object DictionaryLOCK = new object();

        internal static SafeDictionary<string, HTMLScript> Scripts;
        internal static SafeDictionary<string, HTMLScript> ErrorScripts;

        internal static SafeDictionary<string, StaticClass> Statics;
        private static FileSystemWatcher StaticFileWatcher;

        internal static void Initialize()
        {
            try
            {
                var Error500 = new HTMLScript(Path.Combine(Server.ErrorDocsFolder, "500.cshtml"));
                Error500.Compile();

                Scripts = new SafeDictionary<string, HTMLScript>(null);
                ErrorScripts = new SafeDictionary<string, HTMLScript>(Error500);

                CompileErrorPages();
            }
            catch (Exception ex)
            {
                MyLog.Core.Write($"Cannot Find or Compile the Error '500' Script!", LogSeverity.Crash);
                MyLog.Core.Write(ex, LogSeverity.Crash);
                Program.Stop();
                throw;
            }
        }

        private static void CompileErrorPages()
        {
            #region 400
            {
                var Error = "400";
                try
                {
                    var ErrorPage = new HTMLScript(Path.Combine(Server.ErrorDocsFolder, Error + ".cshtml"));
                    ErrorPage.Compile();
                    ErrorScripts.Add(Error, ErrorPage);
                }
                catch (Exception ex)
                {
                    MyLog.Core.Write($"Cannot Find or Compile the Error '{Error}' Script!", LogSeverity.Critical);
                    MyLog.Core.Write(ex.Message, LogSeverity.Critical);
                }
            }
            #endregion

            #region 404
            {
                var Error = "404";
                try
                {
                    var ErrorPage = new HTMLScript(Path.Combine(Server.ErrorDocsFolder, Error + ".cshtml"));
                    ErrorPage.Compile();
                    ErrorScripts.Add(Error, ErrorPage);
                }
                catch (Exception ex)
                {
                    MyLog.Core.Write($"Cannot Find or Compile the Error '{Error}' Script!", LogSeverity.Critical);
                    MyLog.Core.Write(ex.Message, LogSeverity.Critical);
                }
            }
            #endregion 404
        }

        private static DateTime LastWatcherEvent = DateTime.MinValue;
        private static string LastWatcherEventFName = null;
        
        internal static void InitializeStatic()
        {
            Statics = new SafeDictionary<string, StaticClass>(null);

            MyLog.Core.Write("Compiling Static Classes!");

            var files = Directory.GetFiles(Path.Combine(Server.ServerRoot, Settings.StaticFolder));
            Array.Sort(files);
            foreach (var file in files)
            {
                if (file.EndsWith("Start.cs"))
                    continue;

                if (file.EndsWith(".cs"))
                {
                    Statics.Add(file, new StaticClass(file));
                }
            }

            MyLog.Core.Write("Setting up Static Folder Watcher");
            StaticFileWatcher = new FileSystemWatcher(Path.Combine(Server.ServerRoot, Settings.StaticFolder));

            StaticFileWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            StaticFileWatcher.Changed += FileChanged;
            StaticFileWatcher.Created += FileChanged;
            StaticFileWatcher.Deleted += FileChanged;
            StaticFileWatcher.Renamed += FileChanged;

            StaticFileWatcher.Filter = "*.*";
            //watcher.IncludeSubdirectories = true;
            StaticFileWatcher.EnableRaisingEvents = true;

            void FileChanged(object sender, FileSystemEventArgs e)
            {
                // https://stackoverflow.com/questions/449993/vb-net-filesystemwatcher-multiple-change-events/450046#450046
                // There is a nasty bug in the FileSystemWatch which causes the 
                // events of the FileSystemWatcher to be called twice.
                // There are a lot of resources about this to be found on the Internet,
                // but there are no real solutions.
                // Therefore, this workaround is necessary: 
                // If the last time that the event has been raised is only a few msec away, 
                // we ignore it.
                if(LastWatcherEvent != DateTime.MinValue && LastWatcherEventFName != null)
                    if (DateTime.Now.Subtract(LastWatcherEvent).TotalMilliseconds < 500 && LastWatcherEventFName == e.FullPath)
                        return;
                LastWatcherEvent = DateTime.Now;
                LastWatcherEventFName = e.FullPath;


                if (e.FullPath.EndsWith(".cs") == false || e.FullPath.EndsWith("Start.cs"))
                    return;

                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        Statics.Add(e.FullPath, new StaticClass(e.FullPath));
                        break;
                    case WatcherChangeTypes.Changed:
                        Statics[e.FullPath].Recompile();
                        break;
                    case WatcherChangeTypes.Renamed:
                        Statics.Remove(((RenamedEventArgs)e).OldFullPath);
                        Statics.Add(e.FullPath, new StaticClass(e.FullPath));
                        break;
                    case WatcherChangeTypes.Deleted:
                        Statics[e.FullPath].DeleteTempFile();
                        Statics.Remove(e.FullPath);
                        break;
                    default:
                        break;
                }

                //Clear all Script cache due to Static class change.
                //Static classes may affect a large percentage of scripts
                CompiledScripts.Scripts.Clear();
            }
        }

        internal static void ExecuteStartScript()
        {
            MyLog.Core.Write("Executing 'Static/Start.cs'");

            var FilePath = Path.Combine(Server.ServerRoot, Settings.StaticFolder, "Start.cs");

            if (File.Exists(FilePath) == false)
            {
                MyLog.Core.Write("Failed finding File 'Static/Start.cs'", LogSeverity.Warning);
                return;
            }

            //Compile the Script
            HTMLScript Script = new HTMLScript(FilePath);
            Script.Compile(false, false);
            if(Script.CompileError.Length > 0)
            {
                MyLog.Core.Write("Failed compiling 'Static/Start.cshtml' :\n" + Script.CompileError, LogSeverity.Error);
                return;
            }
            try
            {
                //Execute the Script
                Script.Execute(null);
            }
            catch (Exception ex)
            {
                MyLog.Core.Write("Failed executing 'Static/Start.cshtml' :\n" + ex.ToString() , LogSeverity.Error);
            }
        }

        /// <summary>
        /// <para> Compares the Script in the FileSystem with the Script in the Memory.   </para>
        /// <para> If the Script in the FileSystem is different, it will return false           </para>
        /// </summary>
        /// <param name="RealFile">The File Path in the FileSystem</param>
        /// <param name="MemoryFile">The File in the Memory (already Compiled)</param>
        /// <returns>true if both files have the Same Hash</returns>
        internal static bool Compare(string RealFile, HTMLScript MemoryFile)
        {
            var FileData = File.ReadAllText(RealFile);
            string RealHash;

            try
            {
                var ParsedFile = HTMLScriptParser.CombineToCode(HTMLScriptParser.ParseText(FileData, new FileInfo(RealFile).Directory.FullName));
                RealHash = "C_" + Hashing.GetHash(Hashing.SHA1, ParsedFile);
            }
            catch (Exception ex)
            {
                MemoryFile.CompileError = ex.ToString();
                return false;
            }

            

            if (RealHash == MemoryFile.Hash && (RealHash != null && MemoryFile.Hash != null) && MemoryFile.CompileError.Length == 0)
                return true;
            else
                return false;
        }
    }
}
