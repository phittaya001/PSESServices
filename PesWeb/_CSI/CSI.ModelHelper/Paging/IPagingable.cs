using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CSI.ModelHelper.Paging
{
    public interface IPagingable<Tdata, Tcondition>
        where Tdata : class
        where Tcondition : IPagingCriteria
    {
        int Count(Tcondition condition);

        List<Tdata> GetPage(Tcondition condition);
    }
}
