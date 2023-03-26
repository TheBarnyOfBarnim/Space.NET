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
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using SpaceNET.API;

namespace SpaceNET.Scripting.Static
{
    internal class StaticClass
    {
        private string FileName;
        internal Assembly Assembly { get; private set; }
        internal string CompileError { get; private set; }
        internal string CompileErrorRaw { get; private set; }
        internal string TemporaryFile { get; private set; }

        internal StaticClass(string fileName)
        {
            FileName = fileName;
            while (true) //If File currenty in use, wait for it to be unlocked
            {
                try
                {
                    var tuple = StaticCompiler.CompileClass(File.ReadAllText(fileName), fileName);
                    Assembly = tuple.Item1;
                    TemporaryFile = tuple.Item2;
                    CompileError = tuple.Item3;
                    CompileErrorRaw = tuple.Item4;

                    MyLog.Core.Write("Compiled " + Path.GetRelativePath(Server.ServerRoot, FileName) + "\nResult: " + (CompileErrorRaw.Length == 0 ? "Success!" : CompileErrorRaw));
                    return;
                }
                catch (Exception ex){}
            }
        }

        internal void Recompile()
        {
            while (true) //If File currenty in use, wait for it to be unlocked
            {
                try
                {
                    DeleteTempFile();
                    var tuple = StaticCompiler.CompileClass(File.ReadAllText(FileName), FileName);
                    Assembly = tuple.Item1;
                    TemporaryFile = tuple.Item2;
                    CompileError = tuple.Item3;
                    CompileErrorRaw = tuple.Item4;

                    MyLog.Core.Write("Re-Compiled " + Path.GetRelativePath(Server.ServerRoot, FileName) + "\nResult: " + (CompileErrorRaw.Length == 0 ? "Success!" : CompileErrorRaw));
                    return;
                }
                catch (Exception ex) { }
            }
        }

        internal void DeleteTempFile()
        {
            try
            {
                File.Delete(TemporaryFile);
            }
            catch (Exception) { }
        }
    }
}