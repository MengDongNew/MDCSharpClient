using System;
using System.Collections.Generic;
using System.Text;

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
                ValueType type = (ValueType)reader.ReadByte();
                object value = null;//reader.ReadUTF8String();
                switch (type)
                {
                    case ValueType.EnumInt: { value = reader.ReadInt(); } break;
                    case ValueType.Boolean: { value = reader.ReadBool(); } break;
                    case ValueType.Byte: { value = reader.ReadByte(); } break;
                    case ValueType.Int16: { value = reader.ReadShort(); } break;
                    case ValueType.Int32: { value = reader.ReadInt(); } break;
                    case ValueType.Int64: { value = reader.ReadLong(); } break;
                    case ValueType.SByte: { value = reader.ReadSByte(); } break;
                    case ValueType.String: { value = reader.ReadUTF8String(); } break;
                    case ValueType.UInt16: { value = reader.ReaduShort(); } break;
                    case ValueType.UInt32: { value = reader.ReadUint(); } break;
                    case ValueType.UInt64: { value = reader.ReaduLong(); } break;
                }
                //string value = reader.ReadUTF8String();
                operationResponse[parameterCode] = value;
            }

            return operationResponse;
        }
    }
}
