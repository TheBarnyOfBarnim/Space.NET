/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using Space.NET.API;
using Space.NET.API.Utilities;
using Space.NET.Core;
using Space.NET.HTTP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Space.NET.CSharp
{
    internal class HTMLScript
    {
        internal string Hash { get; private set; }
        internal string RealFilePath { get; private set; }
        internal string CompileError = "";
        private MethodInfo Method;

        internal HTMLScript(string FilePath)
        {
            RealFilePath = FilePath;
        }

        internal void Execute(RequestContext Context)
        {
            try
            {
                if (Method == null)
                    throw new Exception();

                var arguments = new object[] { Context.Server, Context.Request, Context.Session, Context.GET, Context.POST, Context.Response };

                Method.Invoke(null, arguments);
            }
            catch (Exception ex)
            {
                Hash = null;
                string Head = (Method == null ? "A compile error occured!" : "A Runtime error occured!");

                Context.Response.Write(
                    "<div style='background-color: black; color: white; font-size: 150%;'><h1>" +
                    Head +
                    "</h1><pre>" +
                    CompileError.Replace("\n", "<br>") +
                    "<br>" +
                    (CompileError.StartsWith("Cannot Parse") ? "" : ex.ToString()) +
                    "<br>&nbsp;" +
                    "</pre></div> <footer style='background-color: black; color: lightgray; padding-left: 10px; padding-top: 10px; padding-bottom: 10px;font-family: \"Courier New\"; position: absolute; width: 99%; bottom: 0px;'> TraceID: "
                    + Context.Request.TraceID +
                    "</footer>");
            }

        }

        internal void Compile()
        {
            string FileData = File.ReadAllText(RealFilePath);
            string ParsedFile = "";

            try
            {
                ParsedFile = HTMLScriptParser.CombineToCode(HTMLScriptParser.ParseText(FileData, new FileInfo(RealFilePath).Directory.FullName));
                Hash = "C_" + API.Utilities.Hashing.GetHash(Hashing.SHA1, ParsedFile);
            }
            catch (Exception ex)
            {
                CompileError = "Cannot Parse File: " + Path.GetRelativePath(HTTPServer.APIServer.DocumentRoot, RealFilePath) + "\n" + ex.ToString();
                Method = null;
                return;
            }

            string Code = "";
            Code += ParsedFile;
            Code += @"void print(object __"+ Hash + "__) { if(__" + Hash + "__ != null){Response.ContentType = \"text/html; charset=utf-8\"; Response.Write(__" + Hash +"__.ToString());}}";

            var CompileResult = HTMLScriptCompiler.CompileScript(Code, ParsedFile, Hash);
            Method = CompileResult.Item1;
            CompileError = CompileResult.Item2;
            if (CompileError.Length > 0)
                Method = null;
        }
    }
}
