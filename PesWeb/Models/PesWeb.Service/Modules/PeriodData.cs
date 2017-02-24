using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class PeriodData
    {
        public int Period_Id { get; set; }

        public string StartDate { get; set; }

        public string FinishDate { get; set; }

        public string Code { get; set; }

        public Nullable<int> Status { get; set; }
    }
}
