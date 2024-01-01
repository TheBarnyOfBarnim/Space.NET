/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2022-2024 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using SpaceNET.API;
using SpaceNET.API.Utilities;
using SpaceNET.HTTP;
using SpaceNET.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SpaceNET.CSharp
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


                if (Context != null)
                {
                    var arguments = new object[] { Context.Request, Context.Session, Context.GET, Context.POST, Context.Response, Context.WebSocket };
                    Method.Invoke(null, arguments);
                }
                else
                    Method.Invoke(null, null);
            }
            catch (Exception ex)
            {
                if (Context == null)
                    throw ex;

                Hash = null;
                string Head = (Method == null ? "A compile error occured!" : "A Runtime error occured!");

                if (Method != null)
                    MyLog.Core.Write(Head + " :\n" + ex);

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

        internal void Compile(bool UseParsing = true, bool UseArguments = true)
        {
            string FileData = File.ReadAllText(RealFilePath);
            string ParsedFile = "";

            if (UseParsing)
            {
                try
                {
                    ParsedFile = HTMLScriptParser.CombineToCode(HTMLScriptParser.ParseText(FileData, new FileInfo(RealFilePath).Directory.FullName));
                }
                catch (Exception ex)
                {
                    CompileError = "Cannot Parse File: " + Path.GetRelativePath(Server.DocumentRoot, RealFilePath) + "\n" + ex.ToString();
                    Method = null;
                    return;
                }
            }
            else
            {
                ParsedFile = FileData;
            }

            Hash = "C_" + API.Utilities.Hashing.GetHash(Hashing.SHA1, ParsedFile);

            string Code = "";
            Code += ParsedFile;
            if (UseArguments)
                Code += @"void print(object __" + Hash + "__) { if(__" + Hash + "__ != null){Response.ContentType = \"text/html; charset=utf-8\"; Response.Write(__" + Hash + "__.ToString());}}";

            var CompileResult = HTMLScriptCompiler.CompileScript(Code, Hash, UseParsing, UseArguments);
            Method = CompileResult.Item1;
            CompileError = CompileResult.Item2;

            MyLog.Core.Write("(Re-)Compiled " + Path.GetRelativePath(Server.ServerRoot, RealFilePath) + "\nResult: " + (CompileError.Length == 0 ? "Success!" : Regex.Replace(CompileError, "<[^>]*>", "")));

            if (CompileError.Length > 0)
                Method = null;
        }
    }
}
