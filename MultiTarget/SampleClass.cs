using System;
using System.Collections.Generic;

#if NET35_OR_ABOVE
using System.Linq;
#endif

using System.Text;

#if NET40_OR_ABOVE
using System.Threading.Tasks;
#endif

namespace Nu.Vs
{
    public class SampleClass
    {
        public static string GetMessage()
        {
            string ver = string.Empty;
#if NET35
            ver = "net 3.5";
#elif NET40
            ver = "net 4.0";
#elif NET45
            ver = "net 4.5";
#elif NET451
            ver = "net 4.5.1";
#endif
            return "MultiTarget of version " + ver;
        }
    }
}
