using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class ProjectMember
    {

        public int SeqID { get; set; }
        //gg
        public string ProjectID { get; set; }

        public short VersionNo { get; set; }

        public int Part2ID { get; set; }

        public string StaffID { get; set; }

        public string StaffName { get; set; }

        public string MemberTypeCode { get; set; }

        public string PositionIncharge { get; set; }

        public string PlanStartDate { get; set; }

        public string PlanFinishDate { get; set; }

        public Nullable<decimal> PlanEffortRate { get; set; }

        public string AcctualStartDate { get; set; }

        public string AcctualFinishDate { get; set; }

        public Nullable<decimal> AcctualEffortRate { get; set; }

        public Nullable<decimal> TransportFee { get; set; }

        public Nullable<decimal> OnSiteAllowance { get; set; }

        public Nullable<decimal> SpecialistFee { get; set; }

        public string OtherExpense { get; set; }

        public Nullable<System.DateTime> OnSiteStart { get; set; }

        public Nullable<System.DateTime> OnSiteFinish { get; set; }

        public string Remark { get; set; }

        public Nullable<bool> IsApproved { get; set; }

        public Nullable<System.DateTime> ApprovedDate { get; set; }

        public System.DateTime UpdateDate { get; set; }

        public Nullable<System.DateTime> CancelDate { get; set; }

        public Nullable<int> version { get; set; }
        public string role { get; set; }
    }
}
