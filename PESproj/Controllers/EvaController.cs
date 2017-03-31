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
using Newtonsoft.Json;

namespace PESproj.Controllers
{
    [RoutePrefix("Eva")]
    public class EvaController : ApiController
    {
        public void insertLog(string Name,string EmployeeNo,string Activity)
        {
            var log = ServiceContainer.GetService<PesWeb.Service.Modules.Log>();
            tblActivityLog lg = new tblActivityLog();
            lg.Activity = Activity;
            lg.EmployeeNo = EmployeeNo;
            lg.Name = Name;
            log.InsertLog(lg);
        }
        
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
            Result.Add(parent);
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
            List<tblEvaluation> evaAll = header.GetAllEvaluation().Where(a => a.EvaluatorNO == Data["EvaluatorNO"].ToString() && a.EmployeeNO == Data["EmployeeNO"].ToString() && a.ProjectNO == Data["ProjectNO"].ToString()).ToList();
            if (evaAll.Count == 0)
            {


                tblEvaluation eva = new tblEvaluation();
                tblProjectMember proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).Where(a => a.StaffID == Data["EmployeeNO"].ToString()).FirstOrDefault();
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
                List<tblEmployee> emp = header.getEmployees();
                int num = header.GetAllApprove().Where(a => a.EvaID == eva_ID).ToList().Count;
                if (num == 0)
                {
                    tblApprove ap = new tblApprove();
                    ap.EvaID = eva_ID;
                    ap.PositionID = header.getEmployees().Where(a => a.EmployeeNo.Replace(" ", "") == Data["EmployeeNO"].ToString().Trim()).FirstOrDefault().PositionNo;
                    ap.Position = header.getPosition().Where(a => a.PositionNo == ap.PositionID).FirstOrDefault().PositionName;
                    tblProjectMember pm = header.getProjectMember().Where(a => a.StaffID.Trim() == Data["EmployeeNO"].ToString().Trim() && a.ProjectID == Data["ProjectNO"].ToString()).FirstOrDefault();
                    string role = header.getPart2Data().Where(a => a.Part2ID == pm.Part2ID).FirstOrDefault().Function;
                    ap.Role = role;
                    ap.Name = pm.StaffName;
                    tblProject pj = header.getProject().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).FirstOrDefault();
                    ap.ProjectCode = pj.CustomerCode + " " + pj.ProjectNameAlias;
                    ap.EmployeeNo = Data["EmployeeNO"].ToString().Trim();
                    int pID = header.insertApprove(ap).ID;
                    List<tblFlowMaster> AllFlow = header.getAllFlow();
                   
                   foreach (tblFlowMaster a in AllFlow)
                    {
                        tblApproveStatus tmp = new tblApproveStatus();
                        tmp.Comment = "";
                        tmp.Status = 0;
                        tmp.FlowOrder = a.Flow;
                        if(a.CodeName == "PM")
                        {
                            tblEmployee emp2 = emp.Where(t => t.EmployeeNo.Trim() == Data["EvaluatorNO"].ToString().Trim()).FirstOrDefault();
                            tmp.Name = emp2.EmployeeFirstName + " " + emp2.EmployeeLastName;
                            tmp.EmployeeNO = emp2.EmployeeNo;
                            tmp.Status = 1;
                        }
                        else if(a.CodeName == "ST")
                        {
                            tmp.Name = pm.StaffName;
                            tmp.EmployeeNO = pm.StaffID;
                        }
                        else if(a.CodeName == "HR")
                        {
                            tblEmployee emp2 = emp.Where(t => t.PositionNo == 23).FirstOrDefault();
                            tmp.Name = emp2.EmployeeFirstName + " " + emp2.EmployeeLastName;
                            tmp.EmployeeNO = emp2.EmployeeNo;
                        }
                        else if(a.CodeName == "GM")
                        {
                            int organNo = (int)emp.Where(s => s.EmployeeNo.Trim() == Data["EmployeeNO"].ToString().Trim()).FirstOrDefault().OrganizationNo;
                            List <tblEmployeeOrganization> emO = header.getEmployeeOrganization().Where(t => t.OrganizationNo == organNo && t.PositionNo == 21).ToList();
                            if(emO.Count != 1)
                            {
                                tmp.Name = "";
                                tmp.EmployeeNO = "";
                            }
                            else
                            {
                                tblEmployee emp2 = emp.Where(t => t.EmployeeNo.Trim() == emO.FirstOrDefault().EmployeeNo).FirstOrDefault();
                                tmp.Name = emp2.EmployeeFirstName + " " + emp2.EmployeeLastName;
                                tmp.EmployeeNO = emp2.EmployeeNo;

                            }
                            
                        }
                        tmp.ApproveID = pID;
                        header.insertApproveStatus(tmp);
                       
                    }
                }
                tblEmployee empLog = emp.Where(t => t.EmployeeNo.Trim() == Data["EvaluatorNO"].ToString().Trim()).FirstOrDefault();
                insertLog(empLog.EmployeeFirstName + " " + empLog.EmployeeLastName, empLog.EmployeeNo, "new evaluation : " + eva_ID);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
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
                if (string.IsNullOrEmpty(Data["Comment"].ToString()))
                {
                    sc.Comment = Data["Comment"].ToString();
                }
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
        public List<JObject> getEvaData(string EvaluatorID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List< SP_GetEvaListByEvaluatorID_Result > evalist = header.getEvaListByEvaluatorID(EvaluatorID).ToList();
            List<JObject> Jeva = new List<JObject>();
            List<tblEmployee> emp = header.getEmployees();
            evalist.ForEach(a =>
            {
                JObject tmp = new JObject();
                tmp["CustomerCompanyAlias"] = a.CustomerCompanyAlias;
                tmp["EmployeeFirstName"] = a.EmployeeFirstName;
                tmp["EmployeeLastName"] = a.EmployeeLastName;
                tmp["EvaluatorFirstName"] = a.EvaluatorFirstName;
                tmp["EvaluatorLastName"] = a.EvaluatorLastName;
                tmp["EvaStatus"] = a.EvaStatus;
                tmp["EvaTerm"] = a.EvaTerm;
                tmp["Eva_ID"] = a.Eva_ID;

                a.StartDatePlan = a.StartDatePlan.Replace(" ", "/");
                if (a.StartDatePlan.ElementAt(4) == '/')
                {
                    a.StartDatePlan = a.StartDatePlan.Substring(0, 4) + "0" + a.StartDatePlan.Substring(5);

                }
                a.StartDatePlan = a.StartDatePlan.Substring(4, 2) + "/" + a.StartDatePlan.Substring(0, 3) + "/" + a.StartDatePlan.Substring(9, 2);
                tmp["StartDatePlan"] = a.StartDatePlan;

                a.FinishDatePlan = a.FinishDatePlan.Replace(" ", "/");
                if (a.FinishDatePlan.ElementAt(4) == '/')
                {
                    a.FinishDatePlan = a.FinishDatePlan.Substring(0, 4) + "0" + a.FinishDatePlan.Substring(5);

                }
                a.FinishDatePlan = a.FinishDatePlan.Substring(4, 2) + "/" + a.FinishDatePlan.Substring(0, 3) + "/" + a.FinishDatePlan.Substring(9, 2);
                tmp["FinishDatePlan"] = a.FinishDatePlan;

                tmp["FinishEvaDate"] = a.FinishEvaDate;
                tmp["Function"] = a.Function;
                tmp["GroupOfStaff"] = a.GroupOfStaff;
                tmp["PeriodID"] = a.PeriodID;
                tmp["ProjectCode"] = a.ProjectCode;
                tmp["ProjectType"] = a.ProjectType;
                tmp["StartEvaDate"] = a.StartEvaDate;
                //tmp["employee_language"] =
               // tblEmployee empTemp = emp.Where(b => b.EmployeeNo.Trim() == a.em).FirstOrDefault();
                tmp["name_language"] = JsonConvert.DeserializeObject<JObject>("{\"EN\":\"" + a.EmployeeFirstName + " " + a.EmployeeLastName + "\",\"TH\":\"" + a.EmployeeFirstNameThai + " " + a.EmployeeLastNameThai + "\"}");
                Jeva.Add(tmp);
            });
            return Jeva;
        }

        [Route("Eva/{EvaluatorID}/{periodID}")]
        [HttpGet]
        public List<JObject> getEvaDataByPeriod(string EvaluatorID,int periodID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<SP_GetEvaListByEvaluatorID_Result> evalist = header.getEvaListByEvaluatorID(EvaluatorID).Where(a => a.PeriodID == periodID).ToList();
            List<JObject> Jeva = new List<JObject>();
            List<tblEmployee> emp = header.getEmployees();
            evalist.ForEach(a =>
            {
                JObject tmp = new JObject();
                tmp["CustomerCompanyAlias"] = a.CustomerCompanyAlias;
                tmp["EmployeeFirstName"] = a.EmployeeFirstName;
                tmp["EmployeeLastName"] = a.EmployeeLastName;
                tmp["EvaluatorFirstName"] = a.EvaluatorFirstName;
                tmp["EvaluatorLastName"] = a.EvaluatorLastName;
                tmp["EvaStatus"] = a.EvaStatus;
                tmp["EvaTerm"] = a.EvaTerm;
                tmp["Eva_ID"] = a.Eva_ID;


                a.StartDatePlan = a.StartDatePlan.Replace(" ", "/");
                if (a.StartDatePlan.ElementAt(4) == '/')
                {
                    a.StartDatePlan = a.StartDatePlan.Substring(0, 4) + "0" + a.StartDatePlan.Substring(5);

                }
                a.StartDatePlan = a.StartDatePlan.Substring(4, 2) + "/" + a.StartDatePlan.Substring(0, 3) + "/" + a.StartDatePlan.Substring(9, 2);
                tmp["StartDatePlan"] = a.StartDatePlan;

                a.FinishDatePlan = a.FinishDatePlan.Replace(" ", "/");
                if (a.FinishDatePlan.ElementAt(4) == '/')
                {
                    a.FinishDatePlan = a.FinishDatePlan.Substring(0, 4) + "0" + a.FinishDatePlan.Substring(5);

                }
                a.FinishDatePlan = a.FinishDatePlan.Substring(4, 2) + "/" + a.FinishDatePlan.Substring(0, 3) + "/" + a.FinishDatePlan.Substring(9, 2);
                tmp["FinishDatePlan"] = a.FinishDatePlan;

                tmp["FinishEvaDate"] = a.FinishEvaDate;
                tmp["Function"] = a.Function;
                tmp["GroupOfStaff"] = a.GroupOfStaff;
                tmp["PeriodID"] = a.PeriodID;
                tmp["ProjectCode"] = a.ProjectCode;
                tmp["ProjectType"] = a.ProjectType;
                tmp["StartEvaDate"] = a.StartEvaDate;
                //tmp["employee_language"] =
                // tblEmployee empTemp = emp.Where(b => b.EmployeeNo.Trim() == a.em).FirstOrDefault();
                tmp["name_language"] = JsonConvert.DeserializeObject<JObject>("{\"EN\":\"" + a.EmployeeFirstName + " " + a.EmployeeLastName + "\",\"TH\":\"" + a.EmployeeFirstNameThai + " " + a.EmployeeLastNameThai + "\"}");
                Jeva.Add(tmp);
            });
            return Jeva;
        }

        [Route("Eva/{EvaluatorID}/History")]
        [HttpGet]
        public List<SP_GetEvaListByEvaluatorID_Result> getEvaListHistory(string EvaluatorID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            return header.getEvaListByEvaluatorID(EvaluatorID).Where(a=>a.EvaStatus==1).ToList();
        }

        [Route("Eva/ApproveHistory/{EmpID}/")]
        [HttpGet]
        public List<JObject> ApproveHistory(string EmpID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();

            // var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<tblApprove> app = header.GetAllApprove();
            List<tblApproveStatus> ApS = header.GetApproveStatus().Where(a => a.EmployeeNO.Trim() == EmpID && a.Status != 0).ToList();
            List<tblApprove> Ap = new List<tblApprove>();
            List<tblEvaluation> ListEva = header.GetAllEvaluation();
            foreach(tblApproveStatus o in ApS)
            {
                tblApprove Approve = app.Where(a => a.ID == o.ApproveID).FirstOrDefault();
                if (Approve != null && ListEva.Where(a=>a.Eva_ID == Approve.EvaID).ToList().Count == 1)
                {
                    Ap.Add(Approve);
                }
                
            }
            List<tblEmployee> emp = header.getEmployees();
            List<JObject> ApObject = new List<JObject>();
            Ap.ForEach(a =>
            {
                JObject tmp = new JObject();
                tmp["ApproverID"] = a.ApproverID;
                tmp["ApproveState"] = a.ApproveState;
                tmp["EvaID"] = a.EvaID;
                tmp["GM"] = a.GM;
                tmp["HR"] = a.HR;
                tmp["ID"] = a.ID;
                tmp["Name"] = a.Name;
                tmp["PM"] = a.PM;
                tmp["Position"] = a.Position;
                tmp["PositionID"] = a.PositionID;
                tmp["ProjectCode"] = a.ProjectCode;
                tmp["Role"] = a.Role;
                tmp["ST"] = a.ST;
                tblEmployee empTemp = emp.Where(b => b.EmployeeNo.Trim() == a.EmployeeNo).FirstOrDefault();
                tmp["name_language"] = JsonConvert.DeserializeObject < JObject > ("{\"EN\":\"" + empTemp.EmployeeFirstName + " " + empTemp.EmployeeLastName + "\",\"TH\":\"" + empTemp.EmployeeFirstNameThai + " " + empTemp.EmployeeLastNameThai + "\"}");
                ApObject.Add(tmp);
            });
            return ApObject;
        }

        [Route("Delete/{EvaID}")]
        [HttpDelete]
        public HttpResponseMessage DeleteData(int EvaID)
        {
            try
            {
                var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
                tblEvaluation eva = header.GetAllEvaluation().Where(a => a.Eva_ID == EvaID).FirstOrDefault();
                header.DeleteEva(EvaID);
                header.DeleteApprove(EvaID);
                tblEmployee emp = header.getEmployees().Where(a => a.EmployeeNo.Trim() == eva.EvaluatorNO).FirstOrDefault();
                insertLog(emp.EmployeeFirstName + " " + emp.EmployeeLastName, eva.EvaluatorNO, "Delete Evaluation : " + EvaID);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
        }
        //GG

        [Route("UpdateLanguage/")]
        [HttpGet]
        public string UpdateDataTble()
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            var header2 = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeader> Hder = header2.GetAllHeader();
            foreach(tblHeader h in Hder)
            {
                if(h.Text_Eng != "-")
                {
                    string Data = "{\"EN\":\"" + h.Text_Eng + "\",\"TH\":\"" + h.Text + "\"}";
                    header.UpdateDataTable(Data, h.H_ID);
                }
                else
                {
                    string Data = "{\"EN\":\"" + h.Text + "\",\"TH\":\"" + h.Text + "\"}";
                    header.UpdateDataTable(Data, h.H_ID);
                }
            }
            return "success";
        }

        [Route("UpdateApprove/")]
        [HttpGet]
        public string UpdateApproveTble()
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            var header2 = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List < tblApprove > Ap = header.getApprove().ToList();
            List<tblEvaluation> eva = header.GetAllEvaluation();
            Ap.ForEach(a =>
            {
                tblEvaluation empno = eva.Where(b => b.Eva_ID == a.EvaID).FirstOrDefault();
                header2.UpdateApprove(a.ID,(empno==null)?"":empno.EmployeeNO );
            });
            return "success";
        }

        [Route("EvaData/{EvaID}")]
        [HttpGet]
        public List<JObject> getEvaDataByEvaID(int EvaID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<SP_GetEvaDataByEvaID_Result> evadata = header.getEvaDataByEvaID(EvaID).ToList();
            List<JObject> Eva = new List<JObject>();
            List<tblEmployee> emp = header.getEmployees();
            evadata.ForEach(a =>
            {
                JObject tmp = new JObject();
                tmp["Part2ID"] = a.Part2ID;
                tmp["GroupOfStaff"] = a.GroupOfStaff;
                tmp["CustomerCompanyAlias"] = a.CustomerCompanyAlias;
                tmp["ProjectCode"] = a.ProjectCode;
                tmp["ProjectType"] = a.ProjectType;
                tmp["EvaTerm"] = a.EvaTerm;
               
                a.StartDatePlan = a.StartDatePlan.Replace(" ","/");
                if (a.StartDatePlan.ElementAt(4)=='/')
                {
                    a.StartDatePlan = a.StartDatePlan.Substring(0, 4) + "0" + a.StartDatePlan.Substring(5);
                    
                }
                a.StartDatePlan = a.StartDatePlan.Substring(4, 2) + "/" + a.StartDatePlan.Substring(0, 3) + "/" + a.StartDatePlan.Substring(9, 2);
                tmp["StartDatePlan"] = a.StartDatePlan;

                a.FinishDatePlan = a.FinishDatePlan.Replace(" ","/");
                if (a.FinishDatePlan.ElementAt(4) == '/')
                {
                    a.FinishDatePlan = a.FinishDatePlan.Substring(0, 4) + "0" + a.FinishDatePlan.Substring(5);
                    
                }
                a.FinishDatePlan = a.FinishDatePlan.Substring(4, 2) + "/" + a.FinishDatePlan.Substring(0, 3) + "/" + a.FinishDatePlan.Substring(9, 2);
                tmp["FinishDatePlan"] = a.FinishDatePlan;
                tmp["Function"] = a.Function;
                tmp["StartTime"] = a.StartTime;
                tmp["FinishTime"] = a.FinishTime;
                tmp["EvaluatorFirstName"] = a.EvaluatorFirstName;
                tmp["EvaluatorLastName"] = a.EvaluatorLastName;
                tmp["Eva_ID"] = a.Eva_ID;
                
                string text = "{\"EN\":\"" + a.EmployeeFirstName + "\",\"TH\":\"" + a.EmployeeFirstNameThai +"\"}";
                tmp["name_language"] = JsonConvert.DeserializeObject<JObject>(text);
                text = "{\"EN\":\"" + a.EmployeeLastName + "\",\"TH\":\"" + a.EmployeeLastNameThai + "\"}";
                tmp["lastname_language"] = JsonConvert.DeserializeObject<JObject>(text);
                //tblEmployee em = emp.Where(b=>b.EmployeeNo.Trim() == a.)
                text = "{\"EN\":\"" + a.EvaluatorFirstName + " " + a.EvaluatorLastName + "\",\"TH\":\"" + a.EvaluatorFirstNameThai +" " + a.EvaluatorLastNameThai + "\"}";
                tmp["Evaluator"] = JsonConvert.DeserializeObject < JObject>(text);
                Eva.Add(tmp);
                //  evadata[i].EmployeeFirstNameThai = 
            });
            return Eva;
        }

        [Route("Approve/{EmpID}/{EvaID}")]
        [HttpGet]
        public List<SP_GetEvaDataByEvaID_Result> getApprove(string EmpID,int EvaID)
        {
            //string EmpIDs = EmpID.ToString();
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            tblEmployee em = header.getEmployees().Where(a => a.EmployeeNo == EmpID).FirstOrDefault();
            if (em != null){
                if(em.PositionNo == 23)
                {

                }
            }
            List<tblEvaluation> eva = header.GetAllEvaluation().Where(a => a.EvaStatus == 1).ToList();
            return header.getEvaDataByEvaID(EvaID).ToList();
        }

        [Route("ApproveList/{EmpID}")]
        [HttpGet]
        public List<JObject> getApproveList(string EmpID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            
            List<tblApproveStatus> AllAps = header.GetApproveStatus();
            List<tblApproveStatus> ApS = AllAps.Where(a => a.EmployeeNO.Trim() == EmpID && a.Status == 0).GroupBy(a => a.ApproveID).Select(a => a.First()).ToList();
            List<tblEvaluation> ListEva = header.GetAllEvaluation();
            List<tblApprove> Ap = new List<tblApprove>();
            List<tblApprove> AllAp = header.GetAllApprove();
            foreach (tblApproveStatus aps in ApS)
            {
                int num = 0;
                List<tblApproveStatus> tmp = AllAps.Where(a => a.ApproveID == aps.ApproveID).ToList();
                foreach(tblApproveStatus n in tmp)
                {
                    if(n.ID == aps.ID)
                    {
                        break;
                    }
                    else if(n.Status==0)
                    {
                        num = 1;
                    }
                }

                if(num==0)
                {
                    tblApprove Approve = AllAp.Where(a => a.ID == aps.ApproveID).FirstOrDefault();
                    if (Approve != null)
                    {
                        tblEvaluation EvaData = ListEva.Where(a => a.Eva_ID == Approve.EvaID).FirstOrDefault();
                        if(EvaData != null && EvaData.EvaStatus == 1)
                            Ap.Add(Approve);
                    }
                }
                    
            }

            List<tblEmployee> emp = header.getEmployees();
            List<JObject> ApObject = new List<JObject>();
            Ap.ForEach(a =>
            {
                JObject tmp = new JObject();
                tmp["ApproverID"] = a.ApproverID;
                tmp["ApproveState"] = a.ApproveState;
                tmp["EvaID"] = a.EvaID;
                tmp["GM"] = a.GM;
                tmp["HR"] = a.HR;
                tmp["ID"] = a.ID;
                tmp["Name"] = a.Name;
                tmp["PM"] = a.PM;
                tmp["Position"] = a.Position;
                tmp["PositionID"] = a.PositionID;
                tmp["ProjectCode"] = a.ProjectCode;
                tmp["Role"] = a.Role;
                tmp["ST"] = a.ST;
                tblEmployee empTemp = emp.Where(b => b.EmployeeNo.Trim() == a.EmployeeNo).FirstOrDefault();
                tmp["name_language"] = JsonConvert.DeserializeObject<JObject>("{\"EN\":\"" + empTemp.EmployeeFirstName + " " + empTemp.EmployeeLastName + "\",\"TH\":\"" + empTemp.EmployeeFirstNameThai + " " + empTemp.EmployeeLastNameThai + "\"}");
                ApObject.Add(tmp);
            });

            return ApObject;
        }

        [Route("ApproveStatus")]
        [HttpPut]
        public void ApproveState([FromBody]JObject Data)
        {
            
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
           
            // var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            tblApprove AllAp = header.GetAllApprove().Where(a => a.EvaID == Convert.ToInt32(Data["EvaID"].ToString())).OrderByDescending(a=>a.ID).FirstOrDefault();
            tblApproveStatus ApS = header.GetApproveStatus().Where(a => a.EmployeeNO.Trim() == Data["EmpID"].ToString() && a.ApproveID == AllAp.ID).OrderByDescending(a=>a.ID).FirstOrDefault();
            ApS.Status = 1;//Convert.ToInt32(Data["Status"].ToString());
            string type = "";
            if(ApS.FlowOrder == 1)
            {
                AllAp.ST = 1;
                type = "Project Manager";
                
            }
            else if(ApS.FlowOrder == 2)
            {
                type = "Staff";
                AllAp.PM = 1;
            }
            else if(ApS.FlowOrder == 3)
            {
                AllAp.GM = 1;
                type = "Group Manager";
            }
            else if(ApS.FlowOrder == 4)
            {
                AllAp.HR = 1;
                type = "Human Resource";
            }
            header.UpdateApproveData(AllAp);
            header.UpdateApproveData(ApS);
            insertLog(AllAp.Name, Data["EmpID"].ToString(), type + " Approve EvaID : " + Data["EvaID"].ToString());
        }
    }
}
