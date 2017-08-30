using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    public class ClientPeer
    {
        private AsynchronousClient asynchronousClient { get; set; }
        /// <summary>
        /// 超时时间，单位ms，默认10000
        /// </summary>
        public int DisconnectTimeout {
            get { return asynchronousClient.Timeoutmilliseconds; }
            set { asynchronousClient.Timeoutmilliseconds = value; }
        }
        
        public IPeerListener PeerListener { get; protected set; }
        public ConnectionProtocol UsedProtocol { get; set; }

        public ClientPeer(IPeerListener listener, ConnectionProtocol protocolType=ConnectionProtocol.Tcp)
        {
            PeerListener = listener;
            UsedProtocol = protocolType;
        }

        public virtual bool Connect(string ip, string port)
        {
            asynchronousClient = AsynchronousClient.Create(SIpPort.Create(ip, port),this, 1);
            
            asynchronousClient.HandleConnect();
            return false;
        }

        public virtual void Service()
        {
            asynchronousClient.Service();
        }

        public virtual void StopThread()
        {
            asynchronousClient.CloseSocket();
        }

        public virtual bool OpCustom(byte customOpCode, Dictionary<byte, object> customOpParameters)
        {
            PacketSend pk = PacketSend.Create(customOpCode);
            foreach (var parameter in customOpParameters)
            {
                pk.Write((byte) parameter.Key);
                pk.Write((string) parameter.Value);
            }
            asynchronousClient.EnqueuePacketSend(pk.ExportArrByte64K());
            return true;
        }
    }
}
