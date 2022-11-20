/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space.NET.Utilities
{
    internal class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public TValue ExceptionValue;

        public SafeDictionary(TValue exceptionValue)
        {
            ExceptionValue = exceptionValue;
        }


        public new TValue this[TKey key]
        {
            get
            {
                if (base.ContainsKey(key))
                    return base[key];
                else
                    return ExceptionValue;
            }
            set
            {
                if (base.ContainsKey(key))
                    base[key] = value;
                else
                    base.Add(key, value);
            }
        }
    }
}
