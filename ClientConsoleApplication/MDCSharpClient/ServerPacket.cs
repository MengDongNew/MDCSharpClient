using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{

    public class ServerPacket
    {
        public enum CodeType
        {
            OperationCode,
            EventCode,
        }

        public bool err;
        public CodeType CType { get; set; }
        public byte Code { get; set; }
       
        public short ReturnCode { get; set; }
        public ushort len;
        public byte state;
        public ArrByteReader arrByteReader { get; set; } //= new ArrByteReader();

        public static ServerPacket Create(ArrByte64K arrByte64K)
        {
            return new ServerPacket(arrByte64K);
        }
        private ServerPacket(ArrByte64K arrByte64K)
        {
            arrByteReader = new ArrByteReader();
            arrByteReader.SetArrByte(arrByte64K);
            len = arrByteReader.ReaduShort();
            CType = (CodeType)arrByteReader.ReadByte();
            Code = arrByteReader.ReadByte();
            ReturnCode = arrByteReader.ReadShort();
            Console.WriteLine("length　=　{0},CodeType={1} Code = {2}", len, CType, Code);

        }
    }
}
