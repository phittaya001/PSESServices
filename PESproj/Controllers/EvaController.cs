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
            List<SP_GetEmployeeListByPeriodID_Result> Emp = new List<SP_GetEmployeeListByPeriodID_Result>();
            Emp = header.getEmpListByPeriod(Period_ID, EmployeeId).ToList();
            for(int i = 0; i < Emp.Count; i++)
            {
                Emp[i].PlanStartDate = Emp[i].PlanStartDate.Substring(0, 10).Replace('/', '-');
                Emp[i].PlanFinishDate = Emp[i].PlanFinishDate.Substring(0, 10).Replace('/', '-');
            }
            return Emp;
        }

        public void InsertEvaDefaultForm()
        {

        }

        public List<tblHeader> FinalHeader(tblHeader parent, List<tblHeader> ListAll)
        {
            List<tblHeader> ListResult = new List<tblHeader>();
            if (ListAll.Where(a => a.Parent == parent.H_ID).ToList().Count == 0)
            {
                ListResult.Add(parent);
                return ListResult;
            }
            List<tblHeader> Result = new List<tblHeader>();
            foreach (tblHeader res in ListAll.Where(a => a.Parent == parent.H_ID).ToList())
            {
                foreach (tblHeader a in FinalHeader(res, ListAll))
                    Result.Insert(Result.Count, a);
            }
            return Result;
        }

        [Route("InsertEva")]
        [HttpPut]
        public HttpResponseMessage InsertEva([FromBody]JObject Data)
        {

                var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
                var header2 = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
                tblEvaluation eva = new tblEvaluation();
                tblProjectMember proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).Where(a=>a.StaffID== Data["EmployeeNO"].ToString()).FirstOrDefault();
                Period p = header.GetPeriod().Where(a => a.Period_Id == Convert.ToInt32(Data["PeriodID"].ToString())).FirstOrDefault();
                eva.EmployeeNO = Data["EmployeeNO"].ToString();
                eva.EvaluatorNO = Data["EvaluatorNO"].ToString();
                eva.Job_ID = proj.Part2ID;
                eva.PeriodID = p.Period_Id;
                eva.period = p.StartDate.ToString().Substring(3, 7);
                eva.ProjectNO = Data["ProjectNO"].ToString();
                eva.StartEvaDate = Convert.ToDateTime(Data["StartDate"].ToString());
                eva.FinishEvaDate = Convert.ToDateTime(Data["FinishDate"].ToString());
                SP_InsertEvaluation_Result evaid = header.InsertEvaData(eva);
                int eva_ID = evaid.Eva_ID;
                
                List<tblHeaderJob> hj = header2.getAllHeaderJob().Where(a => a.PositionNo == proj.Part2ID).ToList();
            List<tblHeader> Ans = new List<tblHeader>();
            List<tblScore> sc = header.GetAllScore();
            List<SP_GetHeaderByPosition_Result> GetHeader = header2.getHeaderByPosition(proj.Part2ID, eva_ID).OrderBy(a => a.H_ID).ToList();
                List<tblHeader> hd = header2.GetAllHeader().ToList();
            tblHeader H_test = new tblHeader();
                H_test.H_ID = 0;
              //  if (GetHeader.Count - GetHeader.Where(a => a.point).ToList().Count < FinalHeader(H_test, hd).ToList().Count)
                    foreach (tblHeaderJob tmpHJ in hj)
                    {
                        foreach (tblHeader hd2 in hd.Where(a => a.H_ID == tmpHJ.H1_ID))
                        {

                            foreach (tblHeader hd3 in FinalHeader(hd2, hd))
                            {
                                if (sc.Where(a => a.Eva_ID == eva_ID && a.H3_ID == hd3.H_ID).ToList().Count == 0)
                                    //if (Ans.Where(a => a.H_ID == hd3.H_ID).ToList().Count == 0)
                                        Ans.Add(hd3);
                            }
                        }
                    }
                foreach (tblHeader h in Ans)
                {
                    header.InsertSCORE(eva_ID, h.H_ID);
                }

            return Request.CreateResponse(HttpStatusCode.OK);
  
        }

        [Route("InsertScore")]
        [HttpPut]
        public HttpResponseMessage InsertScore([FromBody]JObject Data)
        {
            try
            {
                var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
                tblScore sc = new tblScore();

                tblEvaluation eva = new tblEvaluation();
                tblProjectMember proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).Where(a => a.StaffID == Data["EmployeeNO"].ToString()).FirstOrDefault();
                Period p = header.GetPeriod().Where(a => a.Period_Id == Convert.ToInt32(Data["PeriodID"].ToString())).FirstOrDefault();
                sc.Eva_ID = Convert.ToInt32(Data["Eva_ID"].ToString());
                sc.H3_ID = Convert.ToInt32(Data["H_ID"].ToString());
                sc.point = Convert.ToInt32(Data["point"].ToString());
                header.InsertEvaData(eva);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
        }

        [Route("Eva/{EvaluatorID}/All")]
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

        [Route("Eva/{EvaluatorID}/History")]
        [HttpGet]
        public List<SP_GetEvaListByEvaluatorID_Result> getEvaListHistory(string EvaluatorID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            return header.getEvaListByEvaluatorID(EvaluatorID).Where(a=>a.EvaStatus==1).ToList();
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
            
            return header.getEvaDataByEvaID(EvaID).ToList();
        }



    }
}
