using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class EvaluationData
    {
        public int Eva_ID { get; set; }

        public string EmployeeNO { get; set; }
        //gg
        public string EvaluatorNO { get; set; }

        public string Date { get; set; }

        public Nullable<int> Job_ID { get; set; }

        public string ProjectNO { get; set; }

        public string name { get; set; }

        public string period { get; set; }

        public string Role { get; set; }
        public string ProjectName { get; set; }

        public int VersionNO { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string CustumerAlias { get; set; }
    }
}
