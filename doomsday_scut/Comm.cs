using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doomsday_scut
{
    class Comm
    {
        //这个类放一些通用方法
        public static long Time()
        {
            DateTime dt1 = new DateTime(1970, 1, 1);
            TimeSpan ts = DateTime.Now - dt1;
            return (long)ts.TotalMilliseconds;
        }
    }
}
