using System;
using System.Collections.Generic;

#if NET35_OR_ABOVE
using System.Linq;
#endif

using System.Text;

#if NET40_OR_ABOVE
using System.Threading.Tasks;
#endif

namespace Nu.Vs.Demo
{
    public class BaseClass
    {
        public string BaseMessage { get; set; }

        public BaseClass()
        {
            string ver = string.Empty;
#if NET35
            ver = "3.5";
#elif NET40
            ver = "4.0";
#elif NET45
            ver = "4.5";
#elif NET451
            ver = "4.5.1";
#endif
            BaseMessage = "I'm the base class in ver " + ver;
        }        
    }
}
