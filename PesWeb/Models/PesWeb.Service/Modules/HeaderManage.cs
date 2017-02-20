using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Modules
{
    public class HeaderManage
    {
        public List<tblHeaderTop> getAllHeaderTop()
        {
            PSESEntities db = new PSESEntities();
            return db.tblHeaderTop.ToList();
        }
        public List<tblPosition> getJobDetail()
        {
            PSESEntities db = new PSESEntities();
            return db.tblPosition.ToList();
        }
        public List<tblHeaderJob> getAllHeaderJob()
        {
            PSESEntities db = new PSESEntities();
            return db.tblHeaderJob.ToList();
        }
        public List<tblHeaderMid> getAllHeaderMid()
        {
            PSESEntities db = new PSESEntities();
            return db.tblHeaderMid.ToList();
        }
        public List<tblHeaderBot> getAllHeaderBot()
        {
            PSESEntities db = new PSESEntities();
            return db.tblHeaderBot.ToList();
        }
        public List<SP_GetAllHeaderByJobID_Result> getAllHeader()
        {
            PSESEntities db = new PSESEntities();
            return db.SP_GetAllHeaderByJobID().ToList();
        }
        public List<SP_HeaderTopByJobID_Result> getHeaderByJob()
        {
            PSESEntities db = new PSESEntities();
            return db.SP_HeaderTopByJobID().ToList();
        }
        public List<SP_GetHeaderMidByHeaderTopAndJobID_Result> GetHeaderMidByHeaderTopAndJobID(int H1_ID,int JobID)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_GetHeaderMidByHeaderTopAndJobID(H1_ID,JobID).ToList();
        }
    }
}
