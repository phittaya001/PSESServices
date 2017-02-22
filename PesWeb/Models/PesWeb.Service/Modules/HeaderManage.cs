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

        public void DeleteHeaderTop(int H1_ID,int JobID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_DeleteHeaderTop(H1_ID, JobID);
        }
        public void DeleteHeaderMid(int H2_ID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_DeleteHeaderMid(H2_ID);
        }
        public void DeleteHeaderBot(int H3_ID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_DeleteHeaderBot(H3_ID);
        }
        public void InsertHeaderTop(string Text,string Text_Eng,string Alias,int JobID)
        {
            PSESEntities db = new PSESEntities();
            int H1_ID = db.InsertHeaderTop(Text, Text_Eng, Alias);
            db.SP_InsertHeaderJob(JobID, H1_ID);
        }

        public void InsertHeaderMid(string Text, string Text_Eng, int H1_ID, int JobID)
        {
            PSESEntities db = new PSESEntities();
            db.InsertHeaderMid(Text, H1_ID, JobID, Text_Eng);
        }

        public void InsertHeaderBot(string Text,int H2_ID)
        {
            PSESEntities db = new PSESEntities();
            db.InsertHeaderBot(Text, H2_ID);
        }
    }
}
