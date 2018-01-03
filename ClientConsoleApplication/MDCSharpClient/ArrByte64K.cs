using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace MDCSharpClient
{
    public class ArrByte64K
    {
        public const int ByteSize = 1024 * 10;
        public ushort len = 0;
        public byte[] arrByte64K = new byte[ByteSize];
        public ArrByte64K()
        {

        }
        public ArrByte64K(ArrByte64K bytes64k)
        {
            Array.Copy(bytes64k.arrByte64K, 0, arrByte64K, 0, bytes64k.arrByte64K.Length);
        }
        public static ArrByte64K Create()
        {
            return new ArrByte64K();
        }
        public static ArrByte64K Create(ArrByte64K bytes64k)
        {
            return new ArrByte64K(bytes64k);
        }
        public void AppendBytes(byte[] appendBytes, int length)
        {
            lock (arrByte64K)
            {
                len += (ushort)length;
                if (len > ByteSize)
                {//太大了

                    byte[] bytes = arrByte64K;
                    arrByte64K = new byte[len];
                    Array.Copy(bytes, arrByte64K, len - (ushort)length);
                    //					Debug.Log ("太长了XXXXXXXXXXXXXXXXXXXXX");
                    return;
                }
                Array.Copy(appendBytes, 0, arrByte64K, len - (ushort)length, length);
            }

        }
        public void DelBytes(ushort _len, int index = 0)
        {
            _len = len < _len ? len : _len;
            //byte[] tmps = new byte[_len];
            int endIndex = index + _len;
            endIndex = ByteSize < endIndex ? ByteSize : endIndex;
            lock (arrByte64K)
            {
                for (int i = endIndex; i < ByteSize; i++)
                {
                    arrByte64K[i - endIndex] = arrByte64K[i];
                    arrByte64K[i] = 0;
                }
                if (len >= _len)
                    len -= _len;
            }

        }
        /// <summary>
        /// Resets the arr byte64k.
        /// </summary>
        /// <param name="newLength">New length.</param>
        public void ReLengthArrByte64k(int newLength)
        {
            newLength = 1024 * (newLength / 1024 + 1);
            byte[] bytes = arrByte64K;
            arrByte64K = new byte[newLength];
            Array.Copy(bytes, arrByte64K, bytes.Length);
        }
        public void Clear()
        {
            for (int i = 0; i < ByteSize; i++)
            {
                arrByte64K[i] = 0;
            }
            len = 0;
        }
    }
}
