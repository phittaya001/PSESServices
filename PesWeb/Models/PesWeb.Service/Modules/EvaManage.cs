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

        public SP_InsertEvaluation_Result InsertEvaData(tblEvaluation eva)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_InsertEvaluation(eva.ProjectNO, eva.EvaluatorNO, eva.EmployeeNO, eva.Job_ID,eva.period,eva.PeriodID,eva.StartEvaDate,eva.FinishEvaDate,eva.ProjectCode).FirstOrDefault();
        }

        public List<tblEvaluation> getEvaData()
        {
            PSESEntities db = new PSESEntities();
            return db.tblEvaluation.ToList();
        }

        public List<tblPart2Master> getRole()
        {
            PSESEntities db = new PSESEntities();
            return db.tblPart2Master.ToList();
        }

        public List<tblProject> getProject()
        {
            PSESEntities db = new PSESEntities();
            return db.tblProject.ToList();
        }

        public void DeleteScore(int ScoreID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_DeleteScore(ScoreID);
        }
        public void DeleteEva(int EvaID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_DeleteEva(EvaID);
        }

        public void DeleteApprove(int EvaID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_DeleteApproveByEvaID(EvaID);
        }
        //GG
        public List<tblEmployee> getEmployees()
        {
            PSESEntities db = new PSESEntities();
            return db.tblEmployee.ToList();
        }

        public List<tblOrganization> getOrganization()
        {
            PSESEntities db = new PSESEntities();
            return db.tblOrganization.ToList();
        }

        public List<SP_GetEmployeeListByPeriodID_Result> getEmpListByPeriod(int PeriodID,string evaluatorID)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_GetEmployeeListByPeriodID(PeriodID, evaluatorID).ToList();
        }

        public List<SP_GetEvaListByEvaluatorID_Result> getEvaListByEvaluatorID(string evaluatorID)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_GetEvaListByEvaluatorID(evaluatorID).ToList();
        }

        public List<SP_GetEvaDataByEvaID_Result> getEvaDataByEvaID(int EvaID)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_GetEvaDataByEvaID(EvaID).ToList();
        }
        
        public void InsertSCORE(int EvaID,int H_ID)
        {
            PSESEntities DB = new PSESEntities();
            DB.SP_InsertScore(EvaID, H_ID);
        }
        
        public List<tblScore> GetAllScore()
        {
            PSESEntities db = new PSESEntities();
            return db.tblScore.ToList();
        }

        public List<tblEvaluation> GetAllEvaluation()
        {
            PSESEntities db = new PSESEntities();
            return db.tblEvaluation.ToList();
        }

        public List<tblApprove> GetAllApprove()
        {
            PSESEntities db = new PSESEntities();
            return db.tblApprove.ToList();
        }

        public List<tblPosition> getPosition()
        {
            PSESEntities db = new PSESEntities();
            return db.tblPosition.ToList();
        }

        public List<tblPart2Master> getPart2Data()
        {
            PSESEntities db = new PSESEntities();
            return db.tblPart2Master.ToList();
        }
        public SP_InsertApproveState_Result insertApprove(tblApprove ap)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_InsertApproveState(ap.EvaID, ap.Position, ap.PositionID, ap.ProjectCode, ap.Role, ap.Name,ap.EmployeeNo).FirstOrDefault();
             
        }
        public void UpdateEvaluationData(int EvaID,int PositionNo)
        {
            PSESEntities db = new PSESEntities();
            db.SP_UpdateEvaluationData(EvaID, PositionNo);
        }
        public List<tblEmployeeOrganization> getEmployeeOrganization()
        {
            PSESEntities db = new PSESEntities();
            return db.tblEmployeeOrganization.ToList();
        }
        public List<tblApprove> getApprove()
        {
            PSESEntities db = new PSESEntities();
            return db.tblApprove.ToList();
        }
        public void UpdateApproveData(tblApprove ap)
        {
            PSESEntities db = new PSESEntities();
            // tblApprove tmp = GetAllApprove().Where(a => a.ID == ap.ID).FirstOrDefault();
            int number = (int)ap.ApproveState;
            if (ap.GM + ap.HR + ap.PM + ap.ST == 4)
            {
                number = 2;   
            }
            db.SP_UpdateApprove(number, ap.ID, ap.HR, ap.GM, ap.PM, ap.ST);
        }
        public void insertApproveStatus(tblApproveStatus aps)
        {
            PSESEntities db = new PSESEntities();
            db.SP_InsertApproveFlow(aps.Status,aps.FlowOrder,aps.ApproveID,aps.Comment,aps.Name,aps.EmployeeNO);
        }

        public List<tblFlowMaster> getAllFlow()
        {
            PSESEntities db = new PSESEntities();
            return db.tblFlowMaster.ToList();
        }
        public List<tblApproveStatus> GetApproveStatus()
        {
            PSESEntities db = new PSESEntities();
            return db.tblApproveStatus.ToList();
        }

        public void UpdateApproveData(tblApproveStatus aps)
        {
            PSESEntities db = new PSESEntities();
            db.SP_UpdateAprroveData(aps.Status, aps.ID);
        }

        public void UpdateDataTable(string Data,int id)
        {
            PSESEntities db = new PSESEntities();
            db.SP_UpdateDataTable(Data,id);
        }
        public void UpdateGM(int ID,string EmployeeNo,string name)
        {
            PSESEntities db = new PSESEntities();
            db.SP_UpdateApproveStatusFlow(ID, EmployeeNo, name);
        }
        public void updateEvaluationStatus(int evaID,int status)
        {
            PSESEntities db = new PSESEntities();
            db.SP_UpdateEvaluationStatus(evaID, status);
        }

    }
}
