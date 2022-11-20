/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space.NET.API
{
    public class FormFile
    {
        public string FileName { get; private set; }
        public Stream Data { get; private set; }

        internal FormFile(string filename, Stream data)
        {
            FileName = filename;
            Data = data;
        }
    }
}
