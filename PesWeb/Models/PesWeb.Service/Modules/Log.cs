using PesWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PesWeb.Service.Modules
{
    public class Log
    {
        public void InsertLog(tblActivityLog log)
        {
            PSESEntities db = new PSESEntities();
            db.SP_InsertLog(log.Activity, log.EmployeeNo, log.Name);
        }
    }
}