﻿/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Space.NET.API.Utilities
{
    public class IndexerObject
    {
        private Dictionary<string, dynamic> Parameters = new Dictionary<string, dynamic>();

        public dynamic this[string Name]
        {
            get
            {
                if (Parameters.ContainsKey(Name))
                    return Parameters[Name];
                else
                    return null;
            }

            set
            {
                if (Parameters.ContainsKey(Name))
                    Parameters[Name] = value;
                else
                    Parameters.Add(Name, value);
            }
        }

        public override string ToString()
        {
            string output = "";

            foreach (var param in Parameters)
            {
                output += param.Key + " | " + param.Value.ToString() + "\n";
            }

            return output;
        }
    }
}