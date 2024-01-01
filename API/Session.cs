/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using SpaceNET.HTTP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;

namespace SpaceNET.API
{
    public class Session
    {
        private Dictionary<string, dynamic> Parameters = new Dictionary<string, dynamic>();
        public string SessionID { get; private set; }
        public CookieCollection Cookies;

        internal Session(string sessionId, CookieCollection cookies)
        {
            SessionID = sessionId;
            Cookies = cookies;
        }

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

        public void Destroy()
        {
            HTTPServer.Sessions.Remove(this);

            if (Cookies != null)
                Cookies.Add(new Cookie("SessionID", null, "/"));
        }

        public void Start()
        {
            string SessionID = API.Utilities.Hashing.GetHash(SHA256.Create(), Guid.NewGuid().ToString());
            if (Cookies != null)
                Cookies.Add(new Cookie("SessionID", SessionID, "/"));
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