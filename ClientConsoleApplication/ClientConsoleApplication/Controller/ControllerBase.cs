using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDCSharpClient;

namespace ClientConsoleApplication.Controller
{
    abstract class ControllerBase
    {

        protected ControllerBase()
        {
            ClientEngine.Instance.RegistController(OpCode, this);
        }

        public virtual void Destroy()
        {
            ClientEngine.Instance.UnRegistController(OpCode);
        }

        public abstract byte OpCode { get; }
        public abstract void OnOperationResponse(OperationResponse operationResponse);


    }
}
