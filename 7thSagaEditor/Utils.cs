using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.LufiaIIEditor.Etc
{   
    class Utils
    {
        public byte[] byteSubArray(byte[] data, string offset, int length)
        {
            int index = int.Parse(offset, System.Globalization.NumberStyles.HexNumber);
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
