using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class EvaManage
    {
        public List<Period> GetAllPeriod()
        {
            PSESEntities db = new PSESEntities();
            return db.Period.ToList();
        }

        public List<Period> GetPeriod()
        {
            PSESEntities db = new PSESEntities();
            return db.Period.ToList();
        }

        public List<tblProjectMember> getProjectMember()
        {
            PSESEntities db = new PSESEntities();
            return db.tblProjectMember.ToList();
        }

        public void InsertEvaData(tblEvaluation eva)
        {
            PSESEntities db = new PSESEntities();
            db.SP_InsertEvaluation(eva.ProjectNO, eva.EvaluatorNO, eva.EmployeeNO, eva.Job_ID);
        }

        public List<tblEvaluation> getEvaData()
        {
            PSESEntities db = new PSESEntities();
            return db.tblEvaluation.ToList();
        }
    }
}
