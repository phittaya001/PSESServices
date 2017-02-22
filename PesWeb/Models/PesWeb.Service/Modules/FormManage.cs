using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class FormManage
    {
        public int createForm(tblEvaluation form)
        {
            PSESEntities db = new PSESEntities();
            return (int)db.SP_CreateEvaFormAndReturnPrimary(form.EmployeeNO, form.EvaluatorNO, form.Job_ID,form.ProjectNO).FirstOrDefault();
        }

        public int getEvaID(tblEvaluation form)
        {
            PSESEntities db = new PSESEntities();
            return (int)db.getEvaID(form.EvaluatorNO, form.EmployeeNO, form.ProjectNO).FirstOrDefault();
        }

        public void InsertScore(tblScore score)
        {
            PSESEntities db = new PSESEntities();
            db.SP_InsertScore(score.Eva_ID, score.H3_ID);
        }
        
        public List<SP_GetEvaHeaderByEvaID_Result> getEvaDataByEvaID(int EvaID)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_GetEvaHeaderByEvaID(EvaID).ToList();
        }
        
    }
}
