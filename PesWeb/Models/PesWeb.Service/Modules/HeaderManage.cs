using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
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
        public List<tblPart2Master> getJobDetail()
        {
            PSESEntities db = new PSESEntities();
            return db.tblPart2Master.ToList();
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
        public void InsertHeaderJob(int PositionNo,int H_ID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_InsertHeaderJob(PositionNo, H_ID);
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

        public List<tblHeader> getHeaderData()
        {
            PSESEntities db = new PSESEntities();
            return db.tblHeader.ToList();
        }
        public List<tblHeaderLevel> getHeaderLevel()
        {
            PSESEntities db = new PSESEntities();
            return db.tblHeaderLevel.ToList();
        }

        public List<SP_GetHeaderByPosition_Result> getHeaderByPosition(int positionID,int EvaID)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_GetHeaderByPosition(positionID,EvaID).ToList();
        }

        public List<tblHeader> GetAllHeader()
        {
            PSESEntities db = new PSESEntities();
            return db.tblHeader.ToList();
        }

        public void InsertAdditionalHeader(tblHeaderAdditional H)
        {
            PSESEntities db = new PSESEntities();
            db.SP_InsertAdditionalHeader(H.parent, H.Text, H.Text_Eng, H.Eva_ID, H.Alias, H.H_Level,H.Part2ID);
        }

        public List<tblHeaderAdditional> getHeaderAdditional()
        {
            PSESEntities db = new PSESEntities();
            return db.tblHeaderAdditional.ToList();
        }

        public void UpdateScoreData(int EvaID, int point, int H_ID,string comment)
        {
            PSESEntities db = new PSESEntities();
            if (H_ID >= 0)
            {
                db.SP_UpdateData(EvaID, point, H_ID,comment);
            }
            else
            {
                db.SP_UpdateAdditional(EvaID, point, (-1)*H_ID,comment);
            }
            
        }  

        public void insertHeader(tblHeader Header)
        {
            PSESEntities db = new PSESEntities();
            var a = db.SP_InsertHeader(Header.Parent, Header.Text, Header.Text_Eng, Header.Alias, Header.H_Level,0,Header.Text_Language).FirstOrDefault();
            db.SP_InsertHeaderPosition(Header.PositionNo, a.H_id);
            if (Header.H_Level == 1)
            {
                db.SP_InsertHeaderJob(Header.PositionNo,Convert.ToInt32( a.H_id));
            }
        }

        public List<tbllHeaderPosition> getHeaderPosition()
        {
            PSESEntities db = new PSESEntities();
            return db.tbllHeaderPosition.ToList();
        }

        public void DeleteHeader(int H_ID)
        {
            PSESEntities db = new PSESEntities();
            if(H_ID > 0)
            {
                db.SP_DeleteHeader(H_ID);
            }
            else
            {
                db.SP_DeleteHeaderAdditional(H_ID*(-1));
            }
            
        }

        public void UpdateEvaluationStatus(int EvaID,int Status)
        {
            PSESEntities db = new PSESEntities();
            db.SP_UpdateEvaluationStatus(EvaID, Status);
        }

        public void UpdateApprove(int ID, string EmpID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_UpdateApproveDetail(ID, EmpID);
        }
    }
}
