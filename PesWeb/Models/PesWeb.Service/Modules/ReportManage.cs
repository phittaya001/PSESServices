using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class ReportManage
    {
        PSESEntities db = new PSESEntities();
        public List<tblReport> getAllReport()
        {
            return db.tblReport.ToList();
        }
    }
}
