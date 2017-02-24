using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    class EvaluationData
    {
        public int Eva_ID { get; set; }

        public string EmployeeNO { get; set; }

        public string EvaluatorNO { get; set; }

        public Nullable<System.DateTime> Date { get; set; }

        public Nullable<int> Job_ID { get; set; }

        public string ProjectNO { get; set; }

        public string name { get; set; }
    }
}
