/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
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
using Space.NET.API;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text;
using Space.NET.API.Utilities;
using static System.Net.WebRequestMethods;
using System.Text.Json;


namespace Space.NET.CSharp
{
    internal static class HTMLScriptCompiler
    {
        internal static (MethodInfo, string) CompileScript(string Code, string OriginalFile, string Hash)
        {
            string CompileError = "";
            string CodeOnionTop = "using System;" +
                                  "using System.Diagnostics;" +
                                  "using System.Web;" +
                                  "using System.Collections.Generic;" +
                                  "using System.IO;" +
                                  "using System.Data;" +
                                  "using System.Linq;" +
                                  "using System.Text;" +
                                  "using System.Text.Json;" +
                                  "using System.Security.Cryptography;" +
                                  "using System.Threading;" +
                                  "using System.Threading.Tasks;" +
                                  "using System.ComponentModel;" +
                                  "using System.Net.Mail;" +
                                  "using Space.NET.API;" +
                                  "using Space.NET.API.Utilities;";
            CodeOnionTop += "public static class " + Hash + " {";

            CodeOnionTop += "public static void Execute(Server Server, Request Request, Session Session, GET GET, POST POST, Response Response) {";
            string CodeOnionBottom = "}}";

            string NewCode = CodeOnionTop + Code + CodeOnionBottom;

            IEnumerable<string> DefaultNamespaces =
            new[]
            {
                "System",
                "System.Diagnostics",
                "System.Collections.Generic",
                "System.IO",
                "System.Data",
                "System.Linq",
                "System.Threading",
                "System.Threading.Tasks",
                "System.ComponentModel",
                "System.Net.Mail",
                "Space.NET.API",
                "Space.NET.API.Utilities",
            };
            
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(Request).Assembly.Location), //Space.NET.API
                MetadataReference.CreateFromFile(typeof(Hashing).Assembly.Location), //Space.NET.API.Utilities

#region .NET
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IXmlSerializable).Assembly.Location),
                
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

            
            var comp = CSharpCompilation.Create(
                assemblyName: Path.GetFileName("ExternalCode"),
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(NewCode) },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, usings: DefaultNamespaces)
            );


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

                        CompileError += $"Code Snippet [{lineNumber + 1}]:\n";

                        if(lineNumber > 1)
                            CompileError += $"<div style='color:gray;'>    " + HttpUtility.HtmlEncode(NewCode.Split("\n")[lineNumber - 1]) + "</div>";

                        CompileError += $"<div style='color:red;'>" + HttpUtility.HtmlEncode("=>  " + NewCode.Split("\n")[lineNumber]) + "</div>";

                        if (lineNumber < (NewCode.Split('\n').Length - 1))
                            CompileError += $"<div style='color:gray;'>    " + HttpUtility.HtmlEncode(NewCode.Split("\n")[lineNumber+ 1]) + "\n" + "</div>";
                        else
                            CompileError += "\n";

                        CompileError += "</pre><hr>";
                    }

                    return (null, CompileError);
                }
                else
                {
                    Assembly ScriptAssembly = Assembly.Load(ms.ToArray());
                    Type ScriptClass = ScriptAssembly.GetTypes().Where(x => x.Name == Hash).First();
                    MethodInfo ScriptMethod = ScriptClass.GetMethod("Execute");

                    return (ScriptMethod, "");
                }
            }
        }
    }
}

/*


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace C_Sharp_Compile_Test
{
    internal static class PresetCompiler
    {
        internal static Func<dynamic> Compile(string Code)
        {

            
        }
    }
}

*/