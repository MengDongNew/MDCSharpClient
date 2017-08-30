using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    public class OperationResponse
    {
        public string DebugMessage;
        public byte OperationCode;
        public Dictionary<byte, object> Parameters { get; set; }
        public short ReturnCode;

        public OperationResponse()
        {
        }

        public object this[byte parameterCode] {
            get { return Parameters[parameterCode]; }
            set { Parameters[parameterCode] = value; }
        }

        public override string ToString()
        {
            string s = null;
            foreach (var parameter in Parameters)
            {
                s += parameter.Key.ToString() + ":" + parameter.Value.ToString();
                s += ";";
            }
            return s;
        }


    }
}
