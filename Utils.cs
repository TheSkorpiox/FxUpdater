using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxUpdater
{
    public static class Utils
    {
        public static string ToMo(this double size, string format)
        {
            //double dSize = Convert.ToDouble(size);

            return ((size / 1000000).ToString(format));
        }
    }
}
