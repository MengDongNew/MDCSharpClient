using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    public class ServerPacket
    {
        public bool err;
        public ushort OperationCode { get; set; }
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
            len = arrByteReader.ReaduShort();//(ushort)(arrByte64K.arrByte64K[0] * 256 + arrByte64K.arrByte64K[1]);
            OperationCode = arrByteReader.ReaduShort();//(ushort)(arrByte64K.arrByte64K[2] * 256 + arrByte64K.arrByte64K[3]);
            ReturnCode = arrByteReader.ReadShort();
            Console.WriteLine("length　=　{0}, eventId = {1}", len, OperationCode);
            
            //arrByteReader = new ArrByteReader();
            //len = (ushort)(arrByte64K.arrByte64K[0] * 256 + arrByte64K.arrByte64K[1]);
            //OperationCode = (ushort)(arrByte64K.arrByte64K[2] * 256 + arrByte64K.arrByte64K[3]);
            //Console.WriteLine("length　=　{0}, eventId = {1}", len, OperationCode);
            //arrByteReader.SetArrByte(arrByte64K);
        }
    }
}
