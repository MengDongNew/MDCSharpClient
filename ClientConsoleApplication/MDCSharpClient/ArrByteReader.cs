using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    /// <summary>
    ///                  |--------------------------客户端数据--------|
    ///                  |--------------------------Length-----------|
    ///|----4B:ConnId----|---2B:Length---|---1B:CodeType---|----1B:Code---|---2B:ReturnCode---|--Data---|
    /// </summary>
    public class ArrByteReader
    {
        private static Encoding m_UTF8;

        public static Encoding UTF8
        {
            get
            {
                if (m_UTF8 == null)
                    m_UTF8 = new UTF8Encoding(false, false);

                return m_UTF8;
            }
        }

        private ArrByte64K _arrByte;
        private int _readLen;
        public int ReadLen {
            get { return _readLen; }
        }
        public ArrByteReader() { }

        public void SetArrByte(ArrByte64K arrByte)
        {
            _arrByte = arrByte;
            _readLen = 0;//默认从第byte[0]读取数据
            if (_arrByte != null)
            {
                _arrByte.len = (ushort)(_arrByte.arrByte64K[0] * 256 + _arrByte.arrByte64K[1]);
            }

        }

        public byte[] ReadBytes()
        {
            int len = ReaduShort();
            _readLen += len;

            if (_readLen > _arrByte.len) return null;

            byte[] bytes = new byte[len];
            for (int i = 0; i < len; i++)
            {
                bytes[i] = _arrByte.arrByte64K[_readLen - len + i];
            }

            return bytes;
        }
        public byte ReadByte()
        {
            _readLen += 1;
            if (_readLen > _arrByte.len)
                return 0;
            return _arrByte.arrByte64K[_readLen - 1];
        }

        public short ReadShort()
        {
            _readLen += 2;
            if (_readLen > _arrByte.len)
                return 0;
            return (short)((short)(_arrByte.arrByte64K[_readLen - 2]<<8) |
                  ((short)_arrByte.arrByte64K[_readLen - 1]));

        }
        public ushort ReaduShort()
        {
            _readLen += 2;
            if (_readLen > _arrByte.len)
                return 0;
            return GetuShort(_arrByte.arrByte64K, _readLen - 2);
        }
        public int ReadInt()
        {
            _readLen += 4;
            if (_readLen > _arrByte.len)
                return 0;
            return GetInt(_arrByte.arrByte64K, _readLen - 4);
        }
        public ulong ReaduLong()
        {
            _readLen += 8;
            if (_readLen > _arrByte.len)
                return 0;

            return (((ulong)_arrByte.arrByte64K[_readLen - 8]) << 56)
                   | (((ulong)_arrByte.arrByte64K[_readLen - 7]) << 48)
                   | (((ulong)_arrByte.arrByte64K[_readLen - 6]) << 40)
                   | (((ulong)_arrByte.arrByte64K[_readLen - 5]) << 32)
                   | (((ulong)_arrByte.arrByte64K[_readLen - 4]) << 24)
                   | (((ulong)_arrByte.arrByte64K[_readLen - 3]) << 16)
                   | (((ulong)_arrByte.arrByte64K[_readLen - 2]) << 8)
                   | (((ulong)_arrByte.arrByte64K[_readLen - 1]))
                ;
        }
        public long ReadLong()
        {
            _readLen += 8;
            if (_readLen > _arrByte.len)
                return 0;

            return (((long)_arrByte.arrByte64K[_readLen - 8]) << 56)
                   | (((long)_arrByte.arrByte64K[_readLen - 7]) << 48)
                   | (((long)_arrByte.arrByte64K[_readLen - 6]) << 40)
                   | (((long)_arrByte.arrByte64K[_readLen - 5]) << 32)
                   | (((long)_arrByte.arrByte64K[_readLen - 4]) << 24)
                   | (((long)_arrByte.arrByte64K[_readLen - 3]) << 16)
                   | (((long)_arrByte.arrByte64K[_readLen - 2]) << 8)
                   | (((long)_arrByte.arrByte64K[_readLen - 1]))
                ;
        }
        public string ReadUTF8String(bool safeCheck = true)
        {
            ushort l = ReaduShort();
            return ReadUTF8StringSafe(l, safeCheck);
        }
        private bool IsSafeChar(int c)
        {
            return (c >= 0x20 && c < 0xFFFE);
        }
        private string ReadUTF8StringSafe(int fixedLength, bool safeCheck = true)
        {
            if (_readLen + fixedLength > _arrByte.len)
            {
                _readLen += fixedLength;
                return String.Empty;
            }

            int bound = _readLen + fixedLength;

            int count = 0;
            int index = _readLen;
            int start = _readLen;

            while (index < bound && _arrByte.arrByte64K[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value = 0;

            while (_readLen < bound && (value = _arrByte.arrByte64K[_readLen++]) != 0)
                buffer[index++] = (byte)value;

            string s = UTF8.GetString(buffer);

            bool isSafe = true;

            for (int i = 0; isSafe && i < s.Length; ++i)
                isSafe = IsSafeChar((int)s[i]);

            _readLen = start + fixedLength;

            if (isSafe || !safeCheck)
                return s;

            StringBuilder sb = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; ++i)
                if (IsSafeChar((int)s[i]))
                    sb.Append(s[i]);

            return sb.ToString();
        }

        public static ushort GetuShort(byte[] arrByte, int index)
        {
            return (ushort)(((ushort)(arrByte[index]) << 8)
                            | ((ushort)(arrByte[index + 1])));
        }
        public static int GetInt(byte[] arrByte, int index)
        {
            return (((int)arrByte[index]) << 24)
                   | (((int)arrByte[index + 1]) << 16)
                   | (((int)arrByte[index + 2]) << 8)
                   | (((int)arrByte[index + 3]));
        }
    }
}
