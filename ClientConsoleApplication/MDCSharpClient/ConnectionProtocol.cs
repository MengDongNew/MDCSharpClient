using System;
using System.Collections.Generic;
using System.Text;

namespace MDCSharpClient
{
    public enum ConnectionProtocol : byte
    {
        Udp = 0,
        Tcp = 1,
        WebSocket = 4,
        WebSocketSecure = 5
    }
}
