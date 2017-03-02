using CSI.CastleWindsorHelper;
using Newtonsoft.Json.Linq;
using PesWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PesWeb.Service.Modules;

namespace PESproj.Controllers
{
    [RoutePrefix("Eva")]
    public class EvaController : ApiController
    {
        [Route("Period")]
        [HttpGet]
        public List<PeriodData> GetPeriod()
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<Period> GetPeriod = header.GetAllPeriod().ToList();
            List<PeriodData> pd = new List<PeriodData>();
            foreach(Period p in GetPeriod)
            {
                PeriodData newp = new PeriodData();
                newp.Code = p.Code;
                newp.Period_Id = p.Period_Id;
                newp.StartDate = p.StartDate.ToString().Replace('-', '/').Substring(0,10);
                newp.FinishDate = p.FinishDate.ToString().Replace('-', '/').Substring(0, 10);
                newp.Status = p.Status;
                pd.Add(newp);
            }
            return pd;
        }

        [Route("GetEvaList/{EmployeeID}/{Period_ID}")]
        [HttpGet]
        public List<SP_GetEmployeeListByPeriodID_Result> GetEvaList(string EmployeeId,int Period_ID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            //Period pr = header.GetAllPeriod().Where(a => a.Period_Id == Period_ID).FirstOrDefault();
            //List<tblProjectMember> pm = header.getProjectMember().Where(a => a.StaffID == EmployeeId).ToList();
            //List<tblProjectMember> Result_pm = new List<tblProjectMember>();
            //List<ProjectMember> result = new List<ProjectMember>();
            //if (pm.Count > 0)
            //foreach (tblProjectMember epm in pm)
            //{

            //    List<tblProjectMember> pmList = header.getProjectMember().Where(a => a.ProjectID == epm.ProjectID).ToList();
            //        tblProjectMember role = header.getProjectMember().Where(a=>a.StaffID == EmployeeId).FirstOrDefault();
            //        if(role.Part2ID == 30)
            //        foreach (tblProjectMember temp in pmList)
            //    {

            //            tblEvaluation eva = header.getEvaData().Where(a => a.EvaluatorNO == EmployeeId).Where(a => a.EmployeeNO == temp.StaffID).Where(a => a.ProjectNO == temp.ProjectID).FirstOrDefault();
            //        if(eva == null)
            //        if (temp.StaffID != EmployeeId &&((temp.PlanStartDate>=pr.StartDate && temp.PlanStartDate<=pr.FinishDate)||(temp.PlanFinishDate >= pr.StartDate && temp.PlanFinishDate <= pr.FinishDate)|| (temp.PlanFinishDate <= pr.StartDate && temp.PlanFinishDate >= pr.FinishDate)))
            //        {
            //            tblProject p = header.getProject().Where(a => a.ProjectID == temp.ProjectID).FirstOrDefault();
            //            tblPart2Master p2 = header.getRole().Where(a => a.Part2ID == temp.Part2ID).FirstOrDefault();
            //            ProjectMember  resulttemp = new ProjectMember();
            //            tblEmployee emp = header.getEmployees().Where(a => a.EmployeeNo == temp.StaffID).FirstOrDefault();
            //            resulttemp.SeqID = temp.SeqID;
            //            resulttemp.ProjectID = (temp.ProjectID!=null)?temp.ProjectID:"0";
            //            resulttemp.version = temp.VersionNo;
            //            resulttemp.VersionNo = temp.VersionNo;
            //            resulttemp.Part2ID = temp.Part2ID;
            //            resulttemp.StaffName = temp.StaffName;
            //            resulttemp.Firstname = (emp!=null)?emp.EmployeeFirstName:" - ";
            //            resulttemp.Lastname = (emp != null) ? emp.EmployeeLastName:" - ";
            //            resulttemp.StaffID = temp.StaffID;
            //            resulttemp.MemberTypeCode = temp.MemberTypeCode;
            //            resulttemp.PositionIncharge = temp.PositionIncharge;
            //            resulttemp.PlanStartDate = (temp.PlanStartDate!=null)? temp.PlanStartDate.ToString().Replace('-', '/').Substring(0,10):"No Data";
            //            resulttemp.PlanFinishDate = ((temp.PlanStartDate != null) ? temp.PlanStartDate.ToString().Replace('-', '/').Substring(0, 10):"No Data") + " - " + ((temp.PlanFinishDate != null) ? temp.PlanFinishDate.ToString().Replace('-', '/').Substring(0, 10):"No Data");
            //            resulttemp.PlanEffortRate = temp.PlanEffortRate;
            //            resulttemp.AcctualStartDate = ((temp.AcctualStartDate != null) ? temp.AcctualStartDate.ToString().Replace('-', '/').Substring(0, 10):"No Data");
            //            resulttemp.AcctualFinishDate = ((temp.AcctualFinishDate != null) ? temp.AcctualFinishDate.ToString().Replace('-', '/').Substring(0, 10):"No Data");
            //            resulttemp.AcctualEffortRate = temp.AcctualEffortRate;
            //            resulttemp.role = p2.Function;
            //            resulttemp.ProjectName = p.ProjectName;
            //            resulttemp.ProjectNameAlias = p.ProjectNameAlias;
            //            resulttemp.ProjectCode = p.CustomerCompanyAlias + "-" + p.ProjectNameAlias;
            //            result.Add(resulttemp);
            //            Result_pm.Add(temp);
            //        }
            //    }

            //}
            //return result;
            return header.getEmpListByPeriod(Period_ID, EmployeeId);
        }

        [Route("InsertEva")]
        [HttpPut]
        public HttpResponseMessage InsertEva([FromBody]JObject Data)
        {
            try
            {
                var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
                tblEvaluation eva = new tblEvaluation();
                tblProjectMember proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).Where(a=>a.StaffID== Data["EmployeeNO"].ToString()).FirstOrDefault();
                Period p = header.GetPeriod().Where(a => a.Period_Id == Convert.ToInt32(Data["PeriodID"].ToString())).FirstOrDefault();
                eva.EmployeeNO = Data["EmployeeNO"].ToString();
                eva.EvaluatorNO = Data["EvaluatorNO"].ToString();
                eva.Job_ID = proj.Part2ID;
                eva.PeriodID = p.Period_Id;
                eva.period = p.StartDate.ToString().Substring(0, 5);
                eva.ProjectNO = Data["ProjectNO"].ToString();

                header.InsertEvaData(eva);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
        }

        [Route("Eva/All/{EvaluatorID}")]
        [HttpGet]
        public List<SP_GetEvaListByEvaluatorID_Result> getEvaData(string EvaluatorID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            return header.getEvaListByEvaluatorID(EvaluatorID).ToList();
        }

        [Route("Eva/{EvaluatorID}/{periodID}")]
        [HttpGet]
        public List<SP_GetEvaListByEvaluatorID_Result> getEvaDataByPeriod(string EvaluatorID,int periodID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            return header.getEvaListByEvaluatorID(EvaluatorID).Where(a=>a.PeriodID==periodID).ToList();
        }

        [Route("Delete/{EvaID}")]
        [HttpDelete]
        public HttpResponseMessage DeleteData(int EvaID)
        {
            try
            {
                var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
                header.DeleteEva(EvaID);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
        }
        //GG

        [Route("EvaData/{EvaID}")]
        [HttpGet]
        public List<SP_GetEvaDataByEvaID_Result> getEvaDataByEvaID(int EvaID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            //List<EvaluationData> EvaData = new List<EvaluationData>();
            //List<tblEvaluation> Eva = header.getEvaData().Where(a => a.Eva_ID==EvaID).ToList();
            //foreach (tblEvaluation tmp in Eva)
            //{
            //    tblProjectMember mem = header.getProjectMember().Where(a => a.StaffID == tmp.EmployeeNO.Replace("  ", "")).FirstOrDefault();
            //    EvaluationData newEva = new EvaluationData();
            //    tblProject proj = header.getProject().Where(a => a.ProjectID == tmp.ProjectNO).FirstOrDefault();
            //    tblPart2Master p2 = header.getRole().Where(a => a.Part2ID == tmp.Job_ID).FirstOrDefault();
            //    tblEmployee emp = new tblEmployee();
            //    Period p = header.GetPeriod().Where(a => a.Period_Id == tmp.PeriodID).FirstOrDefault();
            //    emp = header.getEmployees().Where(a => a.EmployeeNo.Contains(tmp.EmployeeNO)).FirstOrDefault();
            //    if (emp != null)
            //    {

            //        emp.OrganizationNo = (emp.OrganizationNo != null) ? emp.OrganizationNo : 1;
            //        tblOrganization org = header.getOrganization().Where(a => a.OrganizationNo == (emp.OrganizationNo)).FirstOrDefault();
            //        if(org!=null)
            //        newEva.GroupOfStaff = org.OrganizationAlias;
            //    }
            //    tblEmployee emp2 = new tblEmployee();
            //    emp2 = header.getEmployees().Where(a => a.EmployeeNo.Contains(tmp.EvaluatorNO)).FirstOrDefault();
            //    if (emp2 != null)
            //    {
            //        newEva.evaluatorFirstname = emp2.EmployeeFirstName;
            //        newEva.evaluatorLastname = emp2.EmployeeLastName;
            //    }
            //    newEva.Firstname = (emp != null) ? emp.EmployeeFirstName : " - ";
            //    newEva.Lastname = (emp != null) ? emp.EmployeeLastName : " - ";
            //    newEva.Eva_ID = tmp.Eva_ID;
            //    newEva.CustumerAlias = proj.CustomerCompanyAlias;
            //    newEva.EmployeeNO = tmp.EmployeeNO;
            //    newEva.EvaluatorNO = tmp.EvaluatorNO;
            //    newEva.Date = tmp.Date.ToString().Replace('-', '/').Substring(0, 10);
            //    newEva.Job_ID = tmp.Job_ID;
            //    newEva.ProjectNO = tmp.ProjectNO;
            //    newEva.name = (mem != null) ? mem.StaffName : "null";
            //    newEva.period = (mem != null) ? mem.PlanStartDate.ToString().Replace('-', '/').Substring(0, 10) + " - " + mem.PlanFinishDate.ToString().Replace('-', '/').Substring(0, 10) : "No Data";
            //    newEva.Role = p2.Function;
            //    newEva.ProjectName = proj.ProjectName;
            //    newEva.VersionNO = mem.VersionNo;
            //    newEva.evaTerm = p.StartDate.ToString().Substring(0, 5);
            //    newEva.ProjectCode = proj.CustomerCompanyAlias + "-" + proj.ProjectNameAlias;
            //    newEva.ProjectType = "Man Base";
            //    EvaData.Add(newEva);

            //}
            //return EvaData;
            return header.getEvaDataByEvaID(EvaID).ToList();
        }

    }
}
