using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Tool;
using Common.Modal;
using MDCSharpClient;

namespace ClientConsoleApplication.Controller
{
    class AccountController : ControllerBase
    {
        public static AccountController Instance { get; set; }
        public AccountController():base() {
            Instance = this;
        }

        public override byte OpCode { get { return (byte)OperationCode.Account; } }

        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            SubCode subCode = ParameterTool.GetSubcode(operationResponse.Parameters);
            switch (subCode) {
                case SubCode.AccLogin: {
                        if (operationResponse.ReturnCode == (byte)ReturnCode.Success) {
                            User user = ParameterTool.GetParameter<User>(operationResponse.Parameters, ParameterCode.User);
                            Log("AccLogin.user.Accesstoken=" + user.Accesstoken);
                        }
                        
                    } break;
                case SubCode.AccRegister: {
                        if (operationResponse.ReturnCode == (byte)ReturnCode.Success)
                        {
                            User user = ParameterTool.GetParameter<User>(operationResponse.Parameters, ParameterCode.User);
                            Log("AccRegist.user.Accesstoken=" + user.Accesstoken);
                        }
                    } break;
            }
        }

        public void Login(User user)
        {
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            ParameterTool.AddSubcode(parameters, SubCode.AccLogin);
            ParameterTool.AddParameter<User>(parameters, ParameterCode.User, user);
            foreach (var v in parameters)
            {
                Log("key=" + v.Key + ",value=" + v.Value);
            }
           
            ClientEngine.Instance.SendRequest(OpCode, parameters);
        }


        void Log(string s)
        {
            Console.WriteLine("AccountController:" + s);
        }
    }
    
}
