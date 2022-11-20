﻿/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using Space.NET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space.NET.HTTP
{
    internal class RequestContext
    {
        internal readonly Server Server;
        internal readonly Request Request;
        internal readonly Session Session;
        internal readonly GET GET;
        internal readonly POST POST;
        internal readonly Response Response;

        public RequestContext(Server server, Request request, Session session, GET get, POST post, Response response)
        {
            Server = server;
            Request = request;
            Session = session;
            GET = get;
            POST = post;
            Response = response;
        }
    }
}