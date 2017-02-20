using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PesWeb.Service.Modules
{
    public class UserLogSvr
    {
        public void EmployeeActivityLog(string EmployeeID
                                        , string Activity)
        {
            using (PSESEntities db = new PSESEntities())
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    try
                    {
                        //db.sp_ADM001_EmployeeActivityLog(EmployeeID, Activity);
                        trans.Complete();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
