using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using MDCSharpClient;

namespace ClientConsoleApplication.Controller
{
    class ServerListController:ControllerBase
    {
        private static ServerListController instance;
        public static ServerListController Instance {
            get { return instance; }
        }

        public ServerListController():base()
        {
            instance = this;
        }
        public override byte OpCode {
            get { return (byte) OperationCode.Server; }
        }

        public override void OnEvent(EventData eventData)
        {
            Log("OnEvent: eventData.Code="+eventData.Code);
            foreach (var parameter in eventData.Parameters)
            {
                Log(parameter.Key + ":" + parameter.Value.ToString());
            }
        }

        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            Log("OnOperationResponse=" + operationResponse.OperationCode+" ReturnCode="+operationResponse.ReturnCode);
            foreach (var parameter in operationResponse.Parameters)
            {
                Log(parameter.Key+":"+parameter.Value.ToString());
            }
        }


        public void RequestServerList()
        {
            Dictionary<byte,object> parameters = new Dictionary<byte, object>();
            parameters.Add(10,"Hello 我是客户端！");
            parameters.Add(11,1111);
            ClientEngine.Instance.SendRequest((byte)OperationCode.Server, parameters);
        }

        void Log(string s)
        {
            Console.WriteLine("ServerListController:"+s);
        }
    }
}
