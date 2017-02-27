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
        public List<ProjectMember> GetEvaList(string EmployeeId,int Period_ID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            Period pr = header.GetAllPeriod().Where(a => a.Period_Id == Period_ID).FirstOrDefault();
            List<tblProjectMember> pm = header.getProjectMember().Where(a => a.StaffID == EmployeeId).ToList();
            List<tblProjectMember> Result_pm = new List<tblProjectMember>();
            List<ProjectMember> result = new List<ProjectMember>();

            foreach (tblProjectMember epm in pm)
            {
                List<tblProjectMember> pmList = header.getProjectMember().Where(a => a.ProjectID == epm.ProjectID).ToList();
                foreach(tblProjectMember temp in pmList)
                {
                    if(temp.StaffID != EmployeeId &&((temp.PlanStartDate>pr.StartDate && temp.PlanStartDate<pr.FinishDate)||(temp.PlanFinishDate > pr.StartDate && temp.PlanFinishDate < pr.FinishDate)|| (temp.PlanFinishDate < pr.StartDate && temp.PlanFinishDate > pr.FinishDate)))
                    {
                        tblPart2Master p2 = header.getRole().Where(a => a.Part2ID == temp.Part2ID).FirstOrDefault();
                        ProjectMember  resulttemp = new ProjectMember();
                        resulttemp.SeqID = temp.SeqID;
                        resulttemp.ProjectID = temp.ProjectID;
                        resulttemp.version = temp.VersionNo;
                        resulttemp.VersionNo = temp.VersionNo;
                        resulttemp.Part2ID = temp.Part2ID;
                        resulttemp.StaffName = temp.StaffName;
                        resulttemp.StaffID = temp.StaffID;
                        resulttemp.MemberTypeCode = temp.MemberTypeCode;
                        resulttemp.PositionIncharge = temp.PositionIncharge;
                        resulttemp.PlanStartDate = temp.PlanStartDate.ToString().Replace('-', '/').Substring(0,10);
                        resulttemp.PlanFinishDate = temp.PlanStartDate.ToString().Replace('-', '/').Substring(0, 10) + " - " + temp.PlanFinishDate.ToString().Replace('-', '/').Substring(0, 10);
                        resulttemp.PlanEffortRate = temp.PlanEffortRate;
                        resulttemp.AcctualStartDate = temp.AcctualStartDate.ToString().Replace('-', '/').Substring(0, 10);
                        resulttemp.AcctualFinishDate = temp.AcctualFinishDate.ToString().Replace('-', '/').Substring(0, 10);
                        resulttemp.AcctualEffortRate = temp.AcctualEffortRate;
                        resulttemp.role = p2.Function;
                        result.Add(resulttemp);
                        Result_pm.Add(temp);
                    }
                }
                
            }
            return result;
        }

        [Route("InsertEva")]
        [HttpPut]
        public HttpResponseMessage InsertEva([FromBody]JObject Data)
        {
            try
            {
                var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
                tblEvaluation eva = new tblEvaluation();
                tblProjectMember proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).FirstOrDefault();

                eva.EmployeeNO = Data["EmployeeNO"].ToString();
                eva.EvaluatorNO = Data["EvaluatorNO"].ToString();
                eva.Job_ID = proj.Part2ID;
                eva.ProjectNO = Data["ProjectNO"].ToString();

                header.InsertEvaData(eva);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
        }

        [Route("Eva/{EvaluatorID}")]
        [HttpGet]
        public List<EvaluationData> getEvaData(string EvaluatorID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<EvaluationData> EvaData = new List<EvaluationData>();
            List<tblEvaluation> Eva = header.getEvaData().Where(a => a.EvaluatorNO == EvaluatorID).ToList();
            foreach(tblEvaluation tmp in Eva)
            {
                tblProjectMember mem = header.getProjectMember().Where(a => a.ProjectID == tmp.ProjectNO).FirstOrDefault();
                EvaluationData newEva = new EvaluationData();
                tblPart2Master p2 = header.getRole().Where(a => a.Part2ID == tmp.Job_ID).FirstOrDefault();
                newEva.Eva_ID = tmp.Eva_ID;
                newEva.EmployeeNO = tmp.EmployeeNO;
                newEva.EvaluatorNO = tmp.EvaluatorNO;
                newEva.Date = tmp.Date.ToString().Substring(0,10);
                newEva.Job_ID = tmp.Job_ID;
                newEva.ProjectNO = tmp.ProjectNO;
                newEva.name = mem.StaffName;
                newEva.period = mem.PlanStartDate.ToString().Substring(0,10) + " - " + mem.PlanFinishDate.ToString().Substring(0,10);
                newEva.Role = p2.Function;
                EvaData.Add(newEva);
            }
            return EvaData;
        }

    }
}
