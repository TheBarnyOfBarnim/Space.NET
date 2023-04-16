using Google.Protobuf.Collections;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SpaceNET.API
{
    public class WebSocket
    {
        private HttpListenerContext ListenerContext;

        private WebSocketContext WebSocketContext;
        private System.Net.WebSockets.WebSocket InternalWebSocket;

        public delegate void On_ReceiveText(string Text);
        public event On_ReceiveText OnReceiveText;

        public delegate void On_ReceiveBinary(byte[] Bytes);
        public event On_ReceiveBinary OnReceiveBinary;

        public delegate void On_ClientClose();
        public event On_ClientClose OnClientClose;

        internal WebSocket(HttpListenerContext listenerContext)
        {
            ListenerContext = listenerContext;

            OnReceiveText += (t) => { };
            OnReceiveBinary += (b) => { };
            OnClientClose += () => { };
        }

        public WebSocketState State => (WebSocketState)InternalWebSocket.State;

        public async void Accept()
        {

            var webSocketContext = await ListenerContext.AcceptWebSocketAsync(null);
            WebSocketContext = webSocketContext;
            InternalWebSocket = webSocketContext.WebSocket;

            ArraySegment<Byte> receiveBuffer = new ArraySegment<byte>(new Byte[1024]);
            MemoryStream ms = new MemoryStream();
            while (State == WebSocketState.Open)
            {
                ms.Position = 0;
                WebSocketReceiveResult result;
                var length = 0;
                do
                {
                    try
                    {
                        result = await InternalWebSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        try { OnClientClose();
                            return; } catch (Exception) { return; }
                    }

                    ms.Write(receiveBuffer.Array, receiveBuffer.Offset, result.Count);
                    length += result.Count;
                }
                while (!result.EndOfMessage);
                
                ms.SetLength(length);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var text = Encoding.UTF8.GetString(ms.ToArray());
                    try { OnReceiveText(text); } catch (Exception) {}
                    
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    try { OnReceiveBinary(ms.ToArray()); }
                    catch (Exception) { }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Close();
                    try { OnClientClose(); }
                    catch (Exception) { }
                }

            }
        }

        public async Task<bool> Send(string Text)
        {
            if (State == WebSocketState.Open || State == WebSocketState.CloseReceived)
            {
                var Bytes = Encoding.UTF8.GetBytes(Text);

                await InternalWebSocket.SendAsync(Bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            else
                return false;
        }

        public async Task<bool> Send(byte[] Bytes)
        {
            if (State == WebSocketState.Open || State == WebSocketState.CloseReceived)
            {
                await InternalWebSocket.SendAsync(Bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            return false;
        }

        public async Task<bool> Close()
        {
            if (State != WebSocketState.Closed || State != WebSocketState.Aborted)
            {
                await InternalWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                return true;
            }
            return false;
        }

        public enum WebSocketState
        {
            None = 0,
            Connecting = 1,
            Open = 2,
            CloseSent = 3,
            CloseReceived = 4,
            Closed = 5,
            Aborted = 6
        }

        //public class SocketException : Exception
        //{
        //    internal SocketException
        //}
    }
}
