﻿/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2022-2024 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using System.IO;

namespace SpaceNET.API
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
