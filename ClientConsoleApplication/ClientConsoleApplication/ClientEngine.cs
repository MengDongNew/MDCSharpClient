using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClientConsoleApplication.Controller;
using MDCSharpClient;

namespace ClientConsoleApplication
{
    class ClientEngine:IPeerListener
    {

        private static ClientEngine instance;
        public static ClientEngine Instance {
            get { return instance; }
        }

        private ClientPeer peer { get; set; }
        private Dictionary<byte, ControllerBase> controllers;

        //连接服务器事件
        public delegate void OnConntctToServerEvent();
        public event OnConntctToServerEvent OnConnectToServer;



        public ClientEngine()
        {
            controllers = new Dictionary<byte, ControllerBase>();
            instance = this;
            peer = new ClientPeer(this);
            peer.Connect("127.0.0.1", "26680");
            RegisterAllController();
            
        }
        //UI主线程
        public void Update()
        {
            if(peer!=null)
                peer.Service();
        }
        #region Methord

        private void RegisterAllController()
        {
            Type[] types = Assembly.GetAssembly(typeof(ControllerBase)).GetTypes();
            foreach (var type in types)
            {
                if (type.FullName.EndsWith("Controller"))
                {
                    Activator.CreateInstance(type);
                }
            }
        }
        //注册
        public void RegistController(byte opCode, ControllerBase controller)
        {
            if (!controllers.ContainsKey(opCode))
            {
                controllers.Add(opCode, controller);
            }
            else
            {
                Log("Operation Code Repeat!! code=" + opCode);
            }
        }
        //注销
        public void UnRegistController(byte opCode)
        {
            if (controllers.ContainsKey(opCode))
            {
                controllers.Remove(opCode);
            }
        }

        //发起请求
        public void SendRequest(byte opCode, Dictionary<byte, object> parameters)
        {
            Log("sendrequest to server , opcode : " + opCode);
            peer.OpCustom((byte)opCode, parameters);
        }
        #endregion

        #region Interface
        public void DebugReturn(DebugLevel level, string message)
        {
            Log("DebugReturn:" + level + " " + message);
        }

        public void OnEvent(EventData eventData)
        {
            Log("OnEvent:");
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            Log("OnOperationResponse:" + operationResponse.OperationCode);
            ControllerBase controller;
            if (controllers.TryGetValue(operationResponse.OperationCode, out controller))
            {
                controller.OnOperationResponse(operationResponse);
            }
            else
            {
               Log("Receive a unknown response . OperationCode :" + operationResponse.OperationCode);
            }
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            Log("OnStatusChanged:" + statusCode);
            switch (statusCode)
            {
                case StatusCode.Connect:
                {
                    //isConnected = true;
                    if (OnConnectToServer != null)
                        OnConnectToServer();
                }
                    break;
                default:
                    //isConnected = false;
                    break;
            }
        }


        #endregion


        void Log(string s)
        {
            Console.WriteLine("ClientEngine:"+s);
        }
    }
}
