using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ModelHelper.Paging
{
    public interface IPagingCriteria
    {
        PagingParam PageParam { get; set; }
        SortingParam[] SortParams { get; set; }
    }
}
