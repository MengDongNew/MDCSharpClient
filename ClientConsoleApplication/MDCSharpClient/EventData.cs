using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
     public class EventData
    {
        public byte Code;
        public Dictionary<byte, object> Parameters;

        public EventData()
        {
            
        }

        public object this[byte key] {
            get { return Parameters[key]; }
            set { Parameters[key] = value; }
        }

        public override string ToString()
        {
            string s= null;
            foreach (var parameter in Parameters)
            {
                s += parameter.Key.ToString() + ":" + parameter.Value.ToString();
                s += ";";
            }
            return s;
        }


    }
}
