using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitboard
{
    class BitsCounter
    {
        public int GetBitsCount1(ulong num)
        {
            int count = 0;
            while(num>0)
            {
                count += (int)(num & 1);
                num >>= 1;
            }
            return count;
        }

        public int GetBitsCount2(ulong num)
        {           
            int count = 0;
            while(num > 0)
            {
                num = (num - 1) & num; // инверсия крайнего еденичного бита (из существующих)
                count++;
            }
            return count;
        }

        // предварительно заполняется массив всех вариантов до 256
        int[] counts = new int[256];
        public void PrepareCounter3()
        {
            for(ulong i =0;i<256;i++)
            {
                counts[i] = GetBitsCount2(i);
            }
        }
        public int GetBitsCount3(ulong num)
        {
            int count = 0;
            while (num > 0)
            {                
                count+= counts[num & 0xff];
                num >>= 8;
            }
            return count;
        }
    }
}
