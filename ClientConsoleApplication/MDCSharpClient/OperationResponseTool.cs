using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    class OperationResponseTool
    {
        public static OperationResponse CreateOperationResponse(ArrByte64K arrByte64K)
        {
            ServerPacket serverPacket = ServerPacket.Create(arrByte64K);
            var reader = serverPacket.arrByteReader;

            OperationResponse operationResponse = new OperationResponse();

            operationResponse.OperationCode = (byte)serverPacket.OperationCode;
            operationResponse.ReturnCode = serverPacket.ReturnCode;
            operationResponse.Parameters = new Dictionary<byte, object>();
            while (reader.ReadLen < arrByte64K.len)
            {
                byte parameterCode = reader.ReadByte();
                string value = reader.ReadUTF8String();
                operationResponse[parameterCode] = value;
            }

            return operationResponse;
        }
    }
}
