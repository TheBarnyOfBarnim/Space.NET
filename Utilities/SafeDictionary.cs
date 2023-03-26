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

namespace SpaceNET.Utilities
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
