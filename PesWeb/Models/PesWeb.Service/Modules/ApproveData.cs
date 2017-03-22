using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class ApproveData
    {
        public int Eva_ID { get; set; }
        public Nullable<int> HR { get; set; }
        public Nullable<int> GM { get; set; }
        public Nullable<int> PM { get; set; }
        public Nullable<int> ST { get; set; }
        public string ProjectCode { get; set; }
        public string Duration { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}
