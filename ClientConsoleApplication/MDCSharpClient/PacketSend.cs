using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    /// <summary>
    ///                  |--------------------------客户端发送数据--------|
    ///                  |--------------------------Length-----------|
    ///                  |---2B:Length---|---1B:OperationCode---|--Data---|
    public class PacketSend
    {
        public ArrByte64K _arrByte64K;
        private ushort _i = 3;// 3 个字节开始存储数据 
        
        private PacketSend()
        {
            _arrByte64K = new ArrByte64K();

        }
        public static PacketSend Create(byte operationCode)
        {
            var pk = new PacketSend();
            pk._arrByte64K.arrByte64K[2] = (byte)(operationCode);//第2位存储operationCode
            return pk;
        }

        public PacketSend Write(ushort v)
        {
            if (_i + 2 >= _arrByte64K.arrByte64K.Length)
            {
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                return this;
            }
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 8); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v); _i++;
            return this;
        }
        public PacketSend Write(byte[] v)
        {
            int length = v.Length;
            if (_i + length + sizeof(ushort) >= _arrByte64K.arrByte64K.Length)
            {
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                LengthenArrByte64k(_i + length + 2);
            }
            Write((ushort)length);
            for (int i = _i; i < _i + length; i++)
            {
                _arrByte64K.arrByte64K[i] = v[i - _i];
            }
            _i += (ushort)length;
            return this;
        }
        public PacketSend Write(byte[] v, int length)
        {

            if (_i + length + sizeof(ushort) >= _arrByte64K.arrByte64K.Length)
            {
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                LengthenArrByte64k(_i + length + 2);
            }
            Write((ushort)length);
            for (int i = _i; i < _i + length; i++)
            {
                _arrByte64K.arrByte64K[i] = v[i - _i];
            }
            _i += (ushort)length;
            return this;
        }


        public PacketSend Write(bool v)
        {
            if (v)
                return Write((byte)1);
            else
                return Write((byte)0);
        }
        public PacketSend Write(byte v)
        {
            if (_i + 1 >= _arrByte64K.arrByte64K.Length)
            {
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                return this;
            }
            _arrByte64K.arrByte64K[_i] = (v); _i++;
            return this;
        }
        public PacketSend Write(short v)
        {
            if (_i + 2 >= _arrByte64K.arrByte64K.Length)
            {
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                return this;
            }
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 8); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v); _i++;
            return this;
        }
        public PacketSend Write(int v)
        {
            if (_i + 4 >= _arrByte64K.arrByte64K.Length)
            {
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                return this;
            }
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 24); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 16); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 8); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v); _i++;
            return this;
        }
        public PacketSend Write(ulong v)
        {
            if (_i + 8 >= _arrByte64K.arrByte64K.Length)
            {
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                return this;
            }
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 56); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 48); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 40); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 32); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 24); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 16); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 8); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v); _i++;
            return this;
        }
        public PacketSend Write(long v)
        {
            if (_i + 8 >= _arrByte64K.arrByte64K.Length)
            {
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                return this;
            }
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 56); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 48); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 40); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 32); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 24); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 16); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v >> 8); _i++;
            _arrByte64K.arrByte64K[_i] = (byte)(v); _i++;
            return this;
        }
        public PacketSend Write(string value)
        {
            return WriteUTF8(value);
        }
        public PacketSend WriteUTF8(string value)
        {
            int length = Encoding.UTF8.GetByteCount(value);
            if (_i + length + 2 >= _arrByte64K.arrByte64K.Length)
            {
                LengthenArrByte64k(_i + length + 2);
                //Debug.Log(ArrByteReader.GetuShort(_arrByte64K.arrByte64K, 6).ToString() + " over buff");
                //return this;
            }
            Write((short)length);
            Encoding.UTF8.GetBytes(value, 0, value.Length, _arrByte64K.arrByte64K, (int)_i);
            _i += (ushort)length;
            return this;
        }
        public void LengthenArrByte64k(int newLength)
        {
            _arrByte64K.ReLengthArrByte64k(newLength);
            /*
            newLength = 1024* (newLength/1024+1);
            byte[] bytes = _arrByte64K.arrByte64K;
            _arrByte64K.arrByte64K = new byte[newLength];
            Array.Copy (bytes, _arrByte64K.arrByte64K,bytes.Length);
            */
        }
        public ArrByte64K ExportArrByte64K()
        {
            _arrByte64K.arrByte64K[0] = (byte)((_i) >> 8);
            _arrByte64K.arrByte64K[1] = (byte)((_i));//0，1字节存储字节流的长度
            _arrByte64K.len = _i;
            var ar = _arrByte64K;
            _arrByte64K = null;
            return ar;
        }
    }
}
