/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using Space.NET.API.Utilities;
using Space.NET.Core;
using Space.NET.HTTP;
using Space.NET.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Space.NET.CSharp
{
    internal static class CompiledScripts
    {
        internal static object DictionaryLOCK = new object();
        internal static SafeDictionary<string, HTMLScript> Scripts;
        internal static SafeDictionary<string, HTMLScript> ErrorScripts;

        internal static void Initialize()
        {
            try
            {
                var Error500 = new HTMLScript(Path.Combine(HTTPServer.APIServer.ErrorDocsFolder, "500.cshtml"));
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
                    var ErrorPage = new HTMLScript(Path.Combine(HTTPServer.APIServer.ErrorDocsFolder, Error + ".cshtml"));
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
                    var ErrorPage = new HTMLScript(Path.Combine(HTTPServer.APIServer.ErrorDocsFolder, Error + ".cshtml"));
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

        /// <summary>
        /// <para> Compares the Script in the FileSystem with the Script in the Memory.   </para>
        /// <para> If the Script in the FileSystem is different, it will return false           </para>
        /// </summary>
        /// <param name="RealFile">The File in the FileSystem</param>
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
