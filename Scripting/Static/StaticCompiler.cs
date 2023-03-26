/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CSharp.RuntimeBinder;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;
using System;
using System.Collections.Generic;
using SpaceNET.API;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text;
using SpaceNET.API.Utilities;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Security.Cryptography;
using SpaceNET.Utilities;
using System.Security.Policy;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection.Emit;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using SpaceNET.CSharp;

namespace SpaceNET.Scripting.Static
{
    internal class StaticCompiler
    {
        internal static (Assembly, string, string, string) CompileClass(string Code, string FileName)
        {
            string UsingMethodCode = "using System;" +
                      "using System.Diagnostics;" +
                      "using System.Web;" +
                      "using System.Collections.Generic;" +
                      "using System.IO;" +
                      "using System.Data;" +
                      "using System.Globalization;" +
                      "using System.Linq;" +
                      "using System.Text;" +
                      "using System.Text.Json;" +
                      "using System.Security.Cryptography;" +
                      "using System.Threading;" +
                      "using System.Threading.Tasks;" +
                      "using System.ComponentModel;" +
                      "using System.Net.Mail;" +
                      "using SpaceNET.API;" +
                      "using SpaceNET.API.Utilities;" +
                      "using System.Xml.Linq;" +
                      "using System.Reflection;" +
                      "using System.Runtime.InteropServices;" +
                      "using SpaceNET.Utilities;";

            IEnumerable<string> DefaultNamespaces =
            new[]
            {
                            "System",
                            "System.Diagnostics",
                            "System.Collections.Generic",
                            "System.IO",
                            "System.Data",
                            "System.Globalization",
                            "System.Linq",
                            "System.Threading",
                            "System.Threading.Tasks",
                            "System.ComponentModel",
                            "System.Net.Mail",
                            "SpaceNET.API",
                            "SpaceNET.API.Utilities",
                            "System.Xml.Linq",
                            "System.Reflection",
                            "System.Runtime.InteropServices",
                            "SpaceNET.Utilities",
            
            };




            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            List<MetadataReference> references = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(Request).Assembly.Location), //SpaceNET.API
                MetadataReference.CreateFromFile(typeof(Hashing).Assembly.Location), //SpaceNET.API.Utilities
                MetadataReference.CreateFromFile(typeof(MyLog).Assembly.Location), //SpaceNET.API.Utilities

#region .NET
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IXmlSerializable).Assembly.Location),

                MetadataReference.CreateFromFile(typeof(MD5).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "Microsoft.CSharp.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "Microsoft.VisualBasic.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "Microsoft.VisualBasic.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "Microsoft.Win32.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "Microsoft.Win32.Registry.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "netstandard.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.AppContext.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Buffers.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.Concurrent.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.Immutable.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.NonGeneric.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.Specialized.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ComponentModel.Annotations.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ComponentModel.DataAnnotations.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ComponentModel.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ComponentModel.EventBasedAsync.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ComponentModel.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ComponentModel.TypeConverter.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Console.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Data.Common.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Data.DataSetExtensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Data.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.Contracts.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.Debug.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.DiagnosticSource.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.FileVersionInfo.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.Process.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.StackTrace.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.TextWriterTraceListener.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.Tools.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.TraceSource.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.Tracing.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Drawing.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Drawing.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Dynamic.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Formats.Asn1.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Globalization.Calendars.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Globalization.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Globalization.Extensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.Compression.Brotli.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.Compression.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.Compression.FileSystem.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.Compression.ZipFile.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.FileSystem.AccessControl.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.FileSystem.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.FileSystem.DriveInfo.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.FileSystem.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.FileSystem.Watcher.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.IsolatedStorage.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.MemoryMappedFiles.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.Pipes.AccessControl.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.Pipes.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.UnmanagedMemoryStream.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.Expressions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.Parallel.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.Queryable.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Memory.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Http.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Http.Json.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.HttpListener.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Mail.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.NameResolution.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.NetworkInformation.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Ping.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Requests.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Security.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.ServicePoint.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Sockets.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.WebClient.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.WebHeaderCollection.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.WebProxy.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.WebSockets.Client.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.WebSockets.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Numerics.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Numerics.Vectors.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ObjectModel.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.DispatchProxy.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.Emit.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.Emit.ILGeneration.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.Emit.Lightweight.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.Extensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.Metadata.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.TypeExtensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Resources.Reader.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Resources.ResourceManager.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Resources.Writer.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.CompilerServices.Unsafe.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.CompilerServices.VisualC.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Extensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Handles.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.InteropServices.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.InteropServices.RuntimeInformation.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Intrinsics.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Loader.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Numerics.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Serialization.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Serialization.Formatters.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Serialization.Json.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Serialization.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.Serialization.Xml.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.AccessControl.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Claims.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.Algorithms.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.Cng.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.Csp.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.Encoding.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.OpenSsl.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.X509Certificates.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Principal.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Principal.Windows.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.SecureString.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ServiceModel.Web.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ServiceProcess.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Text.Encoding.CodePages.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Text.Encoding.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Text.Encoding.Extensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Text.Encodings.Web.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Text.Json.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Text.RegularExpressions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Channels.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Overlapped.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Tasks.Dataflow.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Tasks.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Tasks.Extensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Tasks.Parallel.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Thread.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.ThreadPool.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Timer.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Transactions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Transactions.Local.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.ValueTuple.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Web.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Web.HttpUtility.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Windows.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.Linq.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.ReaderWriter.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.Serialization.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.XDocument.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.XmlDocument.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.XmlSerializer.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.XPath.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Xml.XPath.XDocument.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "WindowsBase.dll")),
#endregion .NET
            };


            #region ImportLibraryStatements
            {
                var lines = Code.Split("\n");
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var lineNumber = i;

                    var StatementMatch = Regex.Match(line, @"^\[ImportLibrary\(\"".*\""\)\](\r\n|\r|\n)"); // ^\[ImportLibrary\(\".*\"\)\]\n
                    if (StatementMatch.Success)
                    {
                        var PathMatch = Regex.Match(StatementMatch.Value, @"(?<=^\[ImportLibrary\("")(.*)(?=""\)\](\r\n|\r|\n))"); // (?<=^\[ImportLibrary\(")(.*)(?="\)\](\r\n|\r|\n))
                        if (PathMatch.Success)
                        {
                            Exception ex = null;
                            var path = Path.Combine(Server.StaticFolder, PathMatch.Value);
                            if (System.IO.File.Exists(path))
                            {
                                try
                                {
                                    var a = Assembly.LoadFrom(path);
                                    references.Add(MetadataReference.CreateFromFile(path));
                                    Code = Code.Replace(StatementMatch.Value, "\n");
                                }
                                catch (Exception ex1)
                                {
                                    ex = ex1;
                                }
                            }
                            else
                            {
                                ex = new FileNotFoundException(null, path);
                            }


                            if (ex != null)
                            {
                                var error = "";
                                error += @"<pre style='font-family: ""Courier New"";'>";
                                error += $"{ex.Message}" + "\n";

                                error += $"Code Snippet [{lineNumber}]:\n";

                                if (lineNumber > 0)
                                    error += $"<div style='color:gray;'>    " + HttpUtility.HtmlEncode(Code.Split("\n")[lineNumber - 1]) + "</div>";

                                error += $"<div style='color:red;'>" + HttpUtility.HtmlEncode("=>  " + Code.Split("\n")[lineNumber]) + "</div>";

                                if (lineNumber < (Code.Split('\n').Length - 1))
                                    error += $"<div style='color:gray;'>    " + HttpUtility.HtmlEncode(Code.Split("\n")[lineNumber + 1]) + "\n" + "</div>";
                                else
                                    error += "\n";

                                error += "</pre><hr>";



                                var errorRaw = "";
                                errorRaw += $"{ex.Message}" + "\n";
                                errorRaw += $"Code Snippet [{lineNumber}]:\n";

                                if (lineNumber > 0)
                                    errorRaw += "    " + Code.Split("\n")[lineNumber - 1] + "\n";

                                errorRaw += "=>  " + Code.Split("\n")[lineNumber] + "\n";

                                if (lineNumber < (Code.Split('\n').Length - 1))
                                    errorRaw += "    " + Code.Split("\n")[lineNumber + 1] + "\n";
                                else
                                    errorRaw += "\n";


                                return (null, null, error, errorRaw);
                            }
                        }
                    }
                }
            }
            #endregion

            string NewCode = UsingMethodCode + "\n" + Code;

            foreach (var (filename, staticClass) in CompiledScripts.Statics)
            {
                if (staticClass.CompileErrorRaw != "")
                {
                    return (null, null, staticClass.CompileError, staticClass.CompileErrorRaw);
                }

                try
                {
                    if (new FileInfo(filename).Length != 0)
                    {
                        //AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(staticClass.TemporaryFile));

                        Assembly.LoadFrom(staticClass.TemporaryFile);
                        references.Add(MetadataReference.CreateFromFile(staticClass.TemporaryFile));
                    }
                }
                catch (Exception)
                {

                }
            }


            var TemporaryFile = Path.GetTempFileName();

            var comp = CSharpCompilation.Create(
                assemblyName: Path.GetFileName(TemporaryFile),
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(NewCode) },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, usings: DefaultNamespaces, optimizationLevel: OptimizationLevel.Debug)
            );
            

            string CompileError = "";
            string CompileErrorRaw = ""; 

            using (var ms = new MemoryStream())
            {
                var result = comp.Emit(ms);
                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        var lineNumber = diagnostic.Location.GetLineSpan().StartLinePosition.Line;

                        CompileError += @"<pre style='font-family: ""Courier New"";'>";
                        CompileError += $"{diagnostic.Id} - {diagnostic.GetMessage(CultureInfo.InvariantCulture)}" + "\n";

                        CompileError += $"<b>Static/</b>{Path.GetFileName(FileName)} | Code Snippet [{lineNumber}]:\n";

                        if (lineNumber > 1)
                            CompileError += $"<div style='color:gray;'>    " + HttpUtility.HtmlEncode(NewCode.Split("\n")[lineNumber - 1]) + "</div>";

                        CompileError += $"<div style='color:red;'>" + HttpUtility.HtmlEncode("=>  " + NewCode.Split("\n")[lineNumber]) + "</div>";

                        if (lineNumber < (NewCode.Split('\n').Length - 1))
                            CompileError += $"<div style='color:gray;'>    " + HttpUtility.HtmlEncode(NewCode.Split("\n")[lineNumber + 1]) + "\n" + "</div>";
                        else
                            CompileError += "\n";

                        CompileError += "</pre><hr>";




                        CompileErrorRaw += $"{diagnostic.Id} - {diagnostic.GetMessage(CultureInfo.InvariantCulture)}" + "\n";
                        CompileErrorRaw += $"Static/{Path.GetFileName(FileName)} | Code Snippet [{lineNumber}]:\n";

                        if (lineNumber > 1)
                            CompileErrorRaw += "    " + NewCode.Split("\n")[lineNumber - 1] + "\n";

                        CompileErrorRaw += "=>  " + NewCode.Split("\n")[lineNumber] + "\n";

                        if (lineNumber < (NewCode.Split('\n').Length - 1))
                            CompileErrorRaw += "    " + NewCode.Split("\n")[lineNumber + 1] + "\n";
                        else
                            CompileErrorRaw += "\n";
                    }

                    return (null, null, CompileError, CompileErrorRaw);
                }
                else
                {
                    var arr = ms.ToArray();
                    Assembly ScriptAssembly = Assembly.Load(arr);
                    var generator = new Lokad.ILPack.AssemblyGenerator();

                    // for ad-hoc serialization
                    //var bytes = generator.GenerateAssemblyBytes(ScriptAssembly);

                    // direct serialization to disk
                    generator.GenerateAssembly(ScriptAssembly, TemporaryFile);
                    return (ScriptAssembly, TemporaryFile, "", "");
                }
            }
        }
    }
}
