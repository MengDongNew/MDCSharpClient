using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    class OperationResponseTool
    {
        public static OperationResponse CreateOperationResponse(ServerPacket serverPacket)
        {
            var reader = serverPacket.arrByteReader;

            OperationResponse operationResponse = new OperationResponse();

            operationResponse.OperationCode = serverPacket.Code;
            operationResponse.ReturnCode = serverPacket.ReturnCode;
            operationResponse.Parameters = new Dictionary<byte, object>();
            while (reader.ReadLen < serverPacket.len)
            {
                byte parameterCode = reader.ReadByte();
                string value = reader.ReadUTF8String();
                operationResponse[parameterCode] = value;
            }

            return operationResponse;
        }
    }
}
