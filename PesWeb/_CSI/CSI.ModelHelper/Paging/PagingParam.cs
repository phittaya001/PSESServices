using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CSI.ModelHelper.Paging
{
    public class PagingParam
    {
        public PagingParam()
        {
            this.RowFrom = 0;
            this.RowTo = -1;
        }

        public PagingParam(int rowFrom, int rowTo)
        {
            this.RowFrom = rowFrom;
            this.RowTo = rowTo;
        }

        public int RowFrom { get; set; }

        public int RowTo { get; set; }
    }
}
