﻿using System;
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
            db.SP_InsertEvaluation(eva.ProjectNO, eva.EvaluatorNO, eva.EmployeeNO, eva.Job_ID,eva.period,eva.PeriodID);
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

        public void DeleteEva(int EvaID)
        {
            PSESEntities db = new PSESEntities();
            db.SP_DeleteEva(EvaID);
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

        public List<SP_GetEmployeeListByPeriod1_Result> getEmpListByPeriod(int PeriodID,string evaluatorID)
        {
            PSESEntities db = new PSESEntities();
            return db.SP_GetEmployeeListByPeriod1(PeriodID, evaluatorID).ToList();
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
    }
}
