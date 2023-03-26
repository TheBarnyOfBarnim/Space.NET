/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using SpaceNET.API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpaceNET.API
{
    public class Response
    {
        private HttpListenerResponse HttpResponse;
        public Encoding ContentEncoding
        {
            set
            {
                HttpResponse.ContentEncoding = value;
            }
            get
            {
                if (HttpResponse.ContentEncoding != null)
                    return HttpResponse.ContentEncoding;
                else
                    return Encoding.UTF8;
            }
        }

        public string ContentType
        {
            set
            {
                HttpResponse.ContentType = value;
            }
        }

        public int StatusCode
        {
            set
            {
                HttpResponse.StatusCode = value;
            }
            get
            {
                return HttpResponse.StatusCode;
            }
        }
        public ObservableList<string> Headers = new ObservableList<string>();
        public long ContentLength
        {
            set
            {
                HttpResponse.ContentLength64 = value;
            }
            get
            {
                return HttpResponse.ContentLength64;
            }
        }
        public string Redirect;

        public int CurrentLength { get; private set; }

        internal Response(HttpListenerResponse httpResponse)
        {
            HttpResponse = httpResponse;
            Headers.On_ItemAdded += (i) =>
            {
                HttpResponse.Headers.Add(Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(i))));
            };
            Headers.On_ItemRemoved += (i) =>
            {
                HttpResponse.Headers.Remove(Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(i))));
            };
        }

        public void Write(string str)
        {
            if(str.Length != 0)
                Write(ContentEncoding.GetBytes(str));
        }

        public void Write(byte[] bytes)
        {
            try
            {
                if (bytes.Length != 0)
                {
                    HttpResponse.OutputStream.Write(bytes);
                    CurrentLength += bytes.Length;
                }
            }
            catch (Exception)
            {

            }

        }
    }
}
