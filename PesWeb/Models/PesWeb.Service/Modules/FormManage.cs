using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class FormManage
    {
        public void createForm(tblEvaluation form)
        {
            PSESEntities db = new PSESEntities();
            db.CreateForm(form.EmployeeNO, form.EvaluatorNO, form.Job_ID,form.ProjectNO);
        }

        public int getEvaID(tblEvaluation form)
        {
            PSESEntities db = new PSESEntities();
            return (int)db.getEvaID(form.EvaluatorNO, form.EmployeeNO, form.ProjectNO).FirstOrDefault();
        }
        
        
    }
}
