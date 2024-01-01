/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2022-2024 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using SpaceNET.API;

namespace SpaceNET.HTTP
{
    internal class RequestContext
    {
        //internal readonly Server Server;
        internal readonly Request Request;
        internal readonly Session Session;
        internal readonly GET GET;
        internal readonly POST POST;
        internal readonly WebSocket WebSocket;
        internal readonly Response Response;

        public RequestContext(Request request, Session session, GET get, POST post, WebSocket webSocket, Response response)
        {
            Request = request;
            Session = session;
            GET = get;
            POST = post;
            WebSocket = webSocket;
            Response = response;
        }
    }
}
