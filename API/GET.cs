/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceNET.API
{
    public class GET
    {
        internal static GET Null => new GET(new Dictionary<string, string>(), new Dictionary<string, FormFile>());
        private Dictionary<string, string> Arguments;
        private Dictionary<string, FormFile> Files;

        internal GET(Dictionary<string, string> arguments, Dictionary<string, FormFile> files)
        {
            Arguments = arguments;
            Files = files;
        }

        public dynamic this[string key]
        {
            get
            {
                if (Arguments.ContainsKey(key))
                    return Arguments[key];
                else if (Files.ContainsKey(key))
                    return Files[key];
                else
                    return null;
            }
        }

        public override string ToString()
        {
            string output = "";
            foreach (var item in Arguments)
            {
                output += item.Key + " | " + item.Value + "\n";
            }

            foreach (var file in Files)
            {
                output += file.Key + " | (STREAM)\n";
            }

            return output;
        }
    }
}
