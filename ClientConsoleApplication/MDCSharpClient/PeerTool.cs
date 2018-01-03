using System;
using System.Collections.Generic;

namespace MDCSharpClient
{
    class PeerTool
    {
        public static void DistributePeer(ServerPacket serverPacket,  ClientPeer peer)
        {
            try
            {
                switch (serverPacket.CType)
                {
                    case ServerPacket.CodeType.OperationCode:
                        peer.PeerListener.OnOperationResponse(OperationResponseTool.CreateOperationResponse(serverPacket));
                        break;
                    case ServerPacket.CodeType.EventCode:
                        peer.PeerListener.OnEvent(EventDataTool.CreateEventData(serverPacket));
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
    }
}
