/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using Space.NET.Core;
using Space.NET.HTTP;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Threading;

namespace Space.NET.CSharp
{
    internal static class HTMLScriptParser
    {
        private const string CodeOpenTag = "<cs>";
        private const string CodeCloseTag = "</cs>";

        //[System.Diagnostics.DebuggerHidden]
        internal static List<(bool, string)> ParseText(string Textinput, string FileDirectory)
        {
            //Handle a include(""); statement
            Handle_Include:
            {
                string[] TextLines = Textinput.Split("\n");
                Textinput = "";

                int includesfound = 0;
                for (int i = 0; i < TextLines.Length; i++)
                {
                    var GetIncludeStatement = Regex.Match(TextLines[i], @"(?<!(\/))include\("".*""\);");
                    if (GetIncludeStatement.Success)
                    {
                        includesfound++;
                        var GetPath = Regex.Match(GetIncludeStatement.Value, @"(?<=include\("")(.*)(?=""\);)");
                        if (GetPath.Success)
                        {
                            var path = GetPath.Value;
                            if (path.StartsWith("."))
                            {
                                path = path.Remove(0, 1);
                                path = path.Insert(0, HTTPServer.APIServer.DocumentRoot);
                            }
                            else if (path.StartsWith("__FILE__"))
                            {
                                path = path.Remove(0, "__FILE__".Length);
                                path = path.Insert(0, FileDirectory);
                            }
                            else if (path.StartsWith("__ErrorDocs__"))
                            {
                                path = path.Remove(0, "__ErrorDocs__".Length);
                                path = path.Insert(0, Path.Combine(HTTPServer.APIServer.ServerRoot, "ErrorDocs"));
                            }


                            var includedText = "";
                            includedText += "</csharp>";

                            if (File.Exists(path))
                            {
                                includedText += File.ReadAllText(path);
                            }
                            else
                            {
                                includedText += "404: File not Found (" + path + ")";
                            }
                            includedText += "<csharp>";

                            string BeforeInclude = TextLines[i].Substring(0, GetIncludeStatement.Index);
                            string AfterInclude = TextLines[i].Substring(GetIncludeStatement.Index + GetIncludeStatement.Length);
                            TextLines[i] = BeforeInclude + includedText + AfterInclude;
                        }
                    }

                    Textinput += TextLines[i] + "\n";
                }

                if (includesfound > 0)
                    goto Handle_Include;
            }


            int ReadOffset = 0;
            List<(bool, string)> Parts = new List<(bool, string)>();

            while(ReadOffset < Textinput.Length)
            {
                //Find all Text Till a CodeOpenTag
                var HTMLBlock = Find(Textinput, CodeOpenTag, ReadOffset);

                //If no CodeOpenTag found => File is pure HTML
                if (HTMLBlock == null)
                {
                    var txt = Textinput.Substring(ReadOffset);
                    if(txt != "\n")
                        Parts.Add((false, txt));
                    break;
                }
                else
                {
                    //Found a Code Block

                    //Advancing Reading Position to OpenTag + Tag Length
                    ReadOffset += HTMLBlock.Length + CodeOpenTag.Length;

                    //Formatting Edge Case
                    HTMLBlock = HTMLBlock.Replace("\\" + CodeOpenTag, HttpUtility.HtmlEncode(CodeOpenTag));
                    HTMLBlock = HTMLBlock.Replace("\\" + CodeCloseTag, HttpUtility.HtmlEncode(CodeCloseTag));

                    //Add Found HTML Block to List
                    if (HTMLBlock != "\n")
                        Parts.Add((false, HTMLBlock));

                    //Find all Text Till a CodeCloseTag
                    var CodeBlock = Find(Textinput, CodeCloseTag, ReadOffset);

                    //User did not close a Tag => Syntax Error
                    if (CodeBlock == null)
                    {
                        throw new Exception("Syntax Error: Reached end of File without closing Code Tag");
                    }
                    else
                    {
                        //Found Code Block to List
                        Parts.Add((true, CodeBlock));
                        //Advancing Reading Position to CloseTag + Tag Length
                        ReadOffset += CodeBlock.Length + CodeCloseTag.Length;
                    }
                }
            }

            return Parts;
        }

        private static string Find(string Text, string ToFind, int Offset = 0)
        {

            string OffsetText = Text.Substring(Offset);
            int FoundIndex = OffsetText.IndexOf(ToFind);

            if (FoundIndex == 0)
            {
                return "";
            }
            else if (FoundIndex == -1)
            {
                return null;
            }
            else
            {
                string TextTillFound = OffsetText.Substring(0, FoundIndex);
                if (Text[FoundIndex - 1] != '\\')
                {
                    return TextTillFound;
                }
                else
                {
                    string output = TextTillFound + ToFind;
                    output += Find(Text, ToFind, FoundIndex + ToFind.Length);
                    return output;
                }
            }
        }

        //[System.Diagnostics.DebuggerHidden]
        internal static string CombineToCode(List<(bool, string)> Parts)
        {
            string Code = "";

            foreach (var part in Parts)
            {
                bool Type = part.Item1;
                string Text = part.Item2;

                if(Type == true)
                {
                    Text += "\n";
                    Code += Text;
                }
                else
                {
                    var Lines = Text.Split('\n').Length;
                    Text = Text.Replace(@"""", @"""""");
                    //Text = Text.Replace("\n", @"");

                    Code += @$"print(@""{Text}"");";
                    Code += new string('\n', Lines);
                }
            }

            return Code;
        }
    }
}
