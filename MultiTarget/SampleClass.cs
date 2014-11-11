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
    public class SampleClass : BaseClass
    {
        public string GetMessage()
        {
            return "MultiTarget extends BaseMessage " + BaseMessage;
        }
    }
}
