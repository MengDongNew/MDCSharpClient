using System;
using System.Collections.Generic;
using System.Text;

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

                //添加value的值类型，如int，byte，string，等
                if (parameter.Value.GetType() == typeof(byte))
                {
                    pk.Write((byte)ValueType.Byte);
                    pk.Write((byte)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(string))
                {
                    pk.Write((byte)ValueType.String);
                    pk.Write((string)parameter.Value);
                }
                else if(parameter.Value.GetType().IsEnum) {
                    pk.Write((byte)ValueType.EnumInt);
                    pk.Write((int)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(sbyte))
                {
                    pk.Write((byte)ValueType.SByte);
                    pk.Write((sbyte)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(bool))
                {
                    pk.Write((byte)ValueType.Boolean);
                    pk.Write((bool)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(short))
                {
                    pk.Write((byte)ValueType.Int16);
                    pk.Write((short)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(int))
                {
                    pk.Write((byte)ValueType.Int32);
                    pk.Write((int)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(long))
                {
                    pk.Write((byte)ValueType.Int64);
                    pk.Write((long)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(ushort))
                {
                    pk.Write((byte)ValueType.UInt16);
                    pk.Write((ushort)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(uint))
                {
                    pk.Write((byte)ValueType.UInt32);
                    pk.Write((uint)parameter.Value);
                }
                else if (parameter.Value.GetType() == typeof(ulong))
                {
                    pk.Write((byte)ValueType.UInt64);
                    pk.Write((ulong)parameter.Value);
                }
               

            }
            asynchronousClient.EnqueuePacketSend(pk.ExportArrByte64K());
            return true;
        }
    }
}
