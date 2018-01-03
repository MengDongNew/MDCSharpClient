﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MDCSharpClient
{
    /// <summary>
    /// 服务器类型
    /// </summary>
    public enum EServerType
    {
        /// <summary>
        /// 登陆服务器
        /// </summary>
        Type_Login = 0,

        /// <summary>
        /// 游戏服务器
        /// </summary>
        Type_Game = 2,
        Max,
        Start = Type_Login,
        End = Type_Game
    }
    /// <summary>
    /// 服务器ip和port
    /// </summary>
    public struct SIpPort
    {
        private string ip;
        private string port;
        public string Ip { get { return ip; } }

        public string Port { get { return port; } }

        public static SIpPort Create(string _ip, string _port)
        {
            return new SIpPort(_ip, _port);
        }
        private SIpPort(string _ip, string _port)
        {
            ip = _ip;
            port = _port;
        }

    }
    /// <summary>
    /// Socket State
    /// </summary>
    public class StateObject
    {
        // Client socket.
        public Socket workSocket { get; set; }

        // Size of receive buffer.
        public const int BufferSize = 1024 * 5;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        public StateObject()
        {
        }
    }

    /// <summary>
    /// 单个服务器socket
    /// </summary>
    public class AsynchronousClient
    {
        /// <summary>
        /// 连接服务器类型
        /// </summary>
        private byte type;
        public byte Type { get { return type; } }
        /// <summary>
        /// 连接结束回调
        /// 成功true，失败:false
        /// </summary>
        //public Action<bool> conFinishedCallback = null;
        /// <summary>
        /// socket失败字符串
        /// </summary>
        private string SockErrorStr = null;
        /// <summary>
        /// 异步连接情况
        /// </summary>
        private bool IsconnectSuccess = false;

        /// <summary>
        /// 连接超时时间,默认10s
        /// </summary>
        public int Timeoutmilliseconds { get; set; } // = 10000;//10s

        /// <summary>
        ///  The port string for the remote device.

        /// </summary>
        private string port = null;//测试
        //
        /// <summary>
        /// The ip string for the remote device.
        /// </summary>
        private string ip = null;//孟栋

        // ManualResetEvent instances signal completion.
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote device.
        private ArrByte64K _receiveByte64K = new ArrByte64K();
        private StateObject _receiveState = new StateObject();

        /// <summary>
        /// connect socket
        /// </summary>
        private Socket clientSocket = null;

        private ClientPeer Peer { get; set; }
        //需要发送的数据包
        private Queue<ArrByte64K> QueSendArrByte64Ks { get; set; }

        //连接发生变化的状态队列
        private Queue<StatusCode> QueStatusCodes { get; set; } //= new Queue<StatusCode>();

        public static AsynchronousClient Create(SIpPort sip, ClientPeer peer, byte _type)
        {
            var client = new AsynchronousClient(peer, _type);
            client.SetIpPort(sip.Ip, sip.Port);
            return client;
        }

        private AsynchronousClient(ClientPeer peer, byte _type)
        {
            Timeoutmilliseconds = 10000;
            type = _type;
            QueSendArrByte64Ks = new Queue<ArrByte64K>();
            Peer = peer;
            QueStatusCodes = new Queue<StatusCode>();
        }
        /// <summary>
        /// 设置socket的 ip 和 port
        /// </summary>
        /// <param name="_ip"></param>
        /// <param name="_port"></param>
        public void SetIpPort(string _ip, string _port)
        {
            ip = _ip;
            port = _port;
        }

        /// <summary>
        /// 启动socket
        /// </summary>
        /// <returns></returns>
        private bool StartClient()
        {
            if (ip == null || port == null)
            {
                Log("Error!!!  ip == null || port == null");
                return false;
            }

            /*
            //Unity 
            String newServerIp = "";
            AddressFamily newAddressFamily = AddressFamily.InterNetwork;
            IpV6.getIPType(ip, port, out newServerIp, out newAddressFamily);
            if (!string.IsNullOrEmpty(newServerIp)) { ip = newServerIp; }
            // Establish the remote endpoint for the socket.
            // The name of the remote device is "host.contoso.com".
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
            // Create a TCP/IP socket.
            clientSocket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Log("Socket AddressFamily :" + newAddressFamily.ToString() + "ServerIp:" + ip);
            */



            IPAddress ipa = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(ipa, int.Parse(port));
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            connectDone.Reset();
            try
            {
                // Connect to the remote endpoint.
                clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
                if (connectDone.WaitOne(Timeoutmilliseconds))
                {
                    if (IsconnectSuccess)
                    {
                        return true;
                    }
                    else
                    {
                        TimeOut(StatusCode.TimeoutDisconnect);
                        return false;
                    }
                }
                else
                {
                    TimeOut(StatusCode.TimeoutDisconnect);
                    return false;
                }
            }
            catch (Exception err)
            {
                SockErrorStr = err.ToString();
                return false;
            }
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;
                // Complete the connection.
                client.EndConnect(ar);
                IsconnectSuccess = true;
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                ConnectResult(StatusCode.Connect);
            }

            catch (Exception e)
            {
                IsconnectSuccess = false;
                SockErrorStr = e.ToString();
                ConnectResult(StatusCode.Exception);
                //Console.WriteLine(e.ToString());
            }
            finally
            {
                // Signal that the connection has been made.
                connectDone.Set();
               
                //Receive(clientSocket);
            }
        }
        /// <summary>
        /// 连接成功操作
        /// </summary>
        private void ConnectResult(StatusCode status)
        {
            EnqueueStatusCode(status);
            //Peer.PeerListener.OnStatusChanged(status);
            //if (conFinishedCallback != null)
            {
                //conFinishedCallback(isconnectSuccess);
                /*
                // unity
                Loom.QueueOnMainThread(() =>
                {
                    conFinishedCallback(isconnectSuccess);
                });
                */
            }
        }
        private void Receive(Socket client)
        {
            try
            {
                //StateObject _receiveState = new StateObject();
                _receiveState.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(_receiveState.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), _receiveState);
                //client.BeginReceive(arrByte64K.arrByte64K, 0, arrByte64K.arrByte64K.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), arrByte64K);
                if (!receiveDone.WaitOne(Timeoutmilliseconds))
                {
                    TimeOut(StatusCode.TimeoutDisconnect);
                }
            }
            catch (Exception e)
            {
                SockErrorStr = e.ToString();
                DisconnectedHandler(StatusCode.Disconnect);
                //TimeOut();
            }

        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (!IsconnectSuccess)
                return;

            try
            {
                //Debug.Log("ReceiveCallback...");
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                if (client == null || !client.Connected)
                {
                    Log("ReceiveCallback======!!!!!!client == null || !client.Connected");
                    return;
                }

                // Read data from the remote device.

                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    _receiveByte64K.AppendBytes(state.buffer, bytesRead);
                    //Debug.Log("ReceiveCallback... bytesRead > 0");
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // Signal that all bytes have been received.
                    // Receive the response from the remote device.
                    Receive(clientSocket);

                }
                receiveDone.Set();
            }

            catch (Exception e)
            {
                IsconnectSuccess = false;
                SockErrorStr = e.ToString();
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="pkt">PacketSend</param>
        public void Send(ArrByte64K arrByte64K)
        {
            if (!CheckSocketState())
            {
                DisconnectedHandler(StatusCode.Disconnect);
                return;
            }


            ArrByte64K arrByte = arrByte64K;// pkt.ExportArrByte64K();
            try
            {
                clientSocket.BeginSend(arrByte.arrByte64K, 0, arrByte.len, 0, new AsyncCallback(SendCallback), clientSocket);
                if (!sendDone.WaitOne(Timeoutmilliseconds))
                {
                    TimeOut(StatusCode.TimeoutDisconnect);
                }
                Receive(clientSocket);
            }
            catch (Exception e)
            {
                SockErrorStr = e.ToString();
                DisconnectedHandler(StatusCode.Disconnect);
                //Console.WriteLine(e.ToString());
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            if (!IsconnectSuccess)
                return;
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                SockErrorStr = e.ToString();
                DisconnectedHandler(StatusCode.Disconnect);
                //Console.WriteLine();
            }
        }

        /// <summary>
        /// 连接socket
        /// </summary>
        public void HandleConnect()
        {
            //Thread thread = new Thread(ThreadConnect);
            //thread.Start();
            ThreadConnect();
        }

        private void ThreadConnect()
        {
            Connect();
        }
        /// <summary>
        /// 断线重连函数
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            if (clientSocket != null)
            {
                //关闭socket
                Log("socket");
                if (IsConnect)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }
                clientSocket.Disconnect(true);
                IsconnectSuccess = false;
                clientSocket.Close();
            }

            //创建socket
            return StartClient();
        }

        public bool IsConnect { get { return IsconnectSuccess; } }
        /// <summary>
        /// 关闭服务器连接
        /// </summary>
        public void CloseSocket()
        {
            if (clientSocket != null)
            {
                if (IsConnect)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }
                clientSocket.Close();
                clientSocket = null;
            }
            //clientSocket = null;
            IsconnectSuccess = false;
            connectDone.Reset();
            sendDone.Reset();
            receiveDone.Reset();
            _receiveByte64K.Clear();
        }

        /// <summary>
        /// 断开socket连接
        /// </summary>
        private void ShutDownSocket()
        {
            Log("---------ShutDownSocket--");
            if (clientSocket != null)
            {
                // Release the socket.
                if (IsConnect)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }

                //clientSocket.Disconnect(true);
                clientSocket.Close();
                clientSocket = null;
                IsconnectSuccess = false;
                connectDone.Reset();
                sendDone.Reset();
                receiveDone.Reset();
            }

        }
        /// <summary>
        /// 检测socket的状态
        /// </summary>
        /// <returns></returns>
        private bool CheckSocketState()
        {
            //return true;
            try
            {
                if (clientSocket == null)
                {
                    return false;//StartClient();
                }
                else if (IsconnectSuccess)
                {
                    return true;
                }
                else//已创建套接字，但未connected
                {
                    ShutDownSocket();
                    return false;
                }

            }
            catch (SocketException se)
            {
                SockErrorStr = se.ToString();
                return false;
            }
        }
        /// <summary>
        /// socket由于连接中断(软/硬中断)的后续工作处理器
        /// </summary>
        private void DisconnectedHandler(StatusCode status)
        {
            EnqueueStatusCode(status);
            //Peer.PeerListener.OnStatusChanged(status);
        }

        private void StatusHandler()
        {
            if(QueStatusCodes.Count ==0)return;

            Queue<StatusCode> queueStatusCodes = new Queue<StatusCode>();
            while (QueStatusCodes.Count>0)
            {
                queueStatusCodes.Enqueue(DequeueStatusCode());
            }
            while (queueStatusCodes.Count>0)
            {
                Peer.PeerListener.OnStatusChanged(queueStatusCodes.Dequeue());
            }
            
        }
        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        private void ReceiveHandler()
        {
            int len = _receiveByte64K.arrByte64K[0] * 256 + _receiveByte64K.arrByte64K[1];
            if (len > 0 && _receiveByte64K.len >= len)
            {
                ArrByte64K bytes64k = ArrByte64K.Create();
                bytes64k.AppendBytes(_receiveByte64K.arrByte64K, len);
                _receiveByte64K.DelBytes((ushort)len);
                //ServerPacket serPkt = ServerPacket.Create(bytes64k);
                //App_Event.HandleEvent(serPkt);
                if (Peer.PeerListener != null)
                {
                    ServerPacket serverPacket = ServerPacket.Create(bytes64k);
                    PeerTool.DistributePeer(serverPacket, Peer);
                    
                }
                    
            }
        }

        private void SendHandler()
        {
            Queue<ArrByte64K> queueArrByte64Ks = new Queue<ArrByte64K>();
            lock (QueSendArrByte64Ks)
            {
                while (QueSendArrByte64Ks.Count > 0)
                {
                    queueArrByte64Ks.Enqueue(QueSendArrByte64Ks.Dequeue());

                }
            }
            while (queueArrByte64Ks.Count>0)
            {
                Send(queueArrByte64Ks.Dequeue());
            }
           
        }
        /// <summary>
        /// ui线程更新
        /// </summary>
        public void Service()
        {
            if (clientSocket == null || !clientSocket.Connected)
                return;
            StatusHandler();

            ReceiveHandler();

            SendHandler();
        }


        /// <summary>
        /// 请求超时回调
        /// </summary>
        private void TimeOut(StatusCode statusCode)
        {
            EnqueueStatusCode(statusCode);
            Log("请求超时..." + "IP:" + ip + "  Port:" + port);
        }

        private void EnqueueStatusCode(StatusCode statusCode)
        {
            lock (QueStatusCodes)
            {
                QueStatusCodes.Enqueue(statusCode);
            }
        }

        private StatusCode DequeueStatusCode()
        {
            lock (QueStatusCodes)
            {
                return QueStatusCodes.Dequeue();
            }
        }
        public void EnqueuePacketSend(ArrByte64K arrByte64K)
        {
            lock (QueSendArrByte64Ks)
            {
                QueSendArrByte64Ks.Enqueue(arrByte64K);
            }
            
        }
        private void Log(string s)
        {
            Console.WriteLine(s);
        }

    }
}
