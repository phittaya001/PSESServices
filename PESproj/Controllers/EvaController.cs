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
                //if(ap.Where(a=>a.EvaID == Emp[i].e))
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
            List<tblEvaluation> evaAll = new List<tblEvaluation>();
            if (evaAll.Count == 0)
            {

                int isAdd = 0;
                string name = "";
                tblProjectMember proj = new tblProjectMember();
                if (Data["EmployeeNO"].ToString().Length > 10)
                {
                    
                    isAdd = 1;
                    List<string> nme = Data["EmployeeNO"].ToString().Split('-').ToList();
                    name = nme[1].Trim();
                    tblPart2Master role = header.getPart2Data().Where(a => a.Function.Trim() == Data["Position"].ToString().Trim()).FirstOrDefault();
                    tblProject project = header.getProject().Where(a => a.ProjectNameAlias.Trim() == Data["ProjectNO"].ToString().Trim()).FirstOrDefault();
                    if (project != null)
                    {
                        Data["ProjectNO"] = project.CustomerCompanyAlias + "-" + project.ProjectNameAlias;
                    }
                    proj.Part2ID = role.Part2ID;
                    Data["EmployeeNO"] = Data["EmployeeNO"].ToString().Split('-')[0].Trim();
                }
                else
                {
                    proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).Where(a => a.StaffID == Data["EmployeeNO"].ToString()).FirstOrDefault();
                }
                header.GetAllEvaluation().Where(a => a.EvaluatorNO == Data["EvaluatorNO"].ToString() && a.EmployeeNO == Data["EmployeeNO"].ToString() && a.ProjectNO == Data["ProjectNO"].ToString() && a.PeriodID == Convert.ToInt32(Data["PeriodID"].ToString())).ToList();
                tblEvaluation eva = new tblEvaluation();
                
                Period p = header.GetPeriod().Where(a => a.Period_Id == Convert.ToInt32(Data["PeriodID"].ToString())).FirstOrDefault();
                eva.EmployeeNO = Data["EmployeeNO"].ToString();
                eva.EvaluatorNO = Data["EvaluatorNO"].ToString();
                //eva.Job_ID = (isAdd==1)? Data["ProjectNO"].ToString().Split(':')
                eva.Job_ID = proj.Part2ID;
                eva.PeriodID = p.Period_Id;
                eva.period = p.StartDate.ToString().Substring(3, 7);
                eva.ProjectNO = Data["ProjectNO"].ToString();
                eva.StartEvaDate = Convert.ToDateTime(Data["StartDate"].ToString());
                eva.FinishEvaDate = Convert.ToDateTime(Data["FinishDate"].ToString());
                tblProject pj = new tblProject();
                if (isAdd == 1)
                {
                    pj = header.getProject().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).FirstOrDefault();
                }
                eva.ProjectCode = (isAdd == 1) ? Data["ProjectNO"].ToString() : pj.CustomerCompanyAlias + " " + pj.ProjectNameAlias;
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
                    ap.PositionID = header.getEmployees().Where(a => a.EmployeeNo.Replace(" ", "") == Data["EmployeeNO"].ToString().Replace(" ","")).FirstOrDefault().PositionNo;
                    ap.Position = header.getPosition().Where(a => a.PositionNo == ap.PositionID).FirstOrDefault().PositionName;
                    
                    tblProjectMember pm = header.getProjectMember().Where(a => a.StaffID.Replace(" ","") == Data["EmployeeNO"].ToString().Replace(" ","") && a.ProjectID == Data["ProjectNO"].ToString()).FirstOrDefault();
                    string role = (isAdd==1)? Data["Position"].ToString(): header.getPart2Data().Where(a => a.Part2ID == pm.Part2ID).FirstOrDefault().Function;
                    if (isAdd == 0)
                    {
                        name = pm.StaffName;
                    }
                    ap.Role = role;
                    tblEmployee emptemp  = emp.Where(a => a.EmployeeNo.Trim() == Data["EmployeeNO"].ToString().Trim()).FirstOrDefault();
                    ap.Name = (emptemp != null) ? emptemp.EmployeeFirstName + " " + emptemp.EmployeeLastName : "";
                    //tblProject pj = header.getProject().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).FirstOrDefault();
                    ap.ProjectCode = eva.ProjectCode;
                    ap.EmployeeNo = Data["EmployeeNO"].ToString().Replace(" ","");
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
                            tblEmployee emp2 = emp.Where(t => t.EmployeeNo.Replace(" ","") == Data["EvaluatorNO"].ToString().Replace(" ","")).FirstOrDefault();
                            tmp.Name = emp2.EmployeeFirstName + " " + emp2.EmployeeLastName;
                            tmp.EmployeeNO = emp2.EmployeeNo;
                           // tmp.Status = 1;
                        }
                        else if(a.CodeName == "ST")
                        {
                            tmp.Name = name;
                            tmp.EmployeeNO = Data["EmployeeNO"].ToString().Replace(" ", "");
                        }
                        else if(a.CodeName == "HR")
                        {
                            tblEmployee emp2 = emp.Where(t => t.PositionNo == 23).FirstOrDefault();
                            tmp.Name = emp2.EmployeeFirstName + " " + emp2.EmployeeLastName;
                            tmp.EmployeeNO = emp2.EmployeeNo;
                        }
                        else if(a.CodeName == "GM")
                        {
                            int organNo = (int)emp.Where(s => s.EmployeeNo.Replace(" ","") == Data["EmployeeNO"].ToString().Replace(" ","")).FirstOrDefault().OrganizationNo;
                            List <tblEmployeeOrganization> emO = header.getEmployeeOrganization().Where(t => t.OrganizationNo == organNo && t.PositionNo == 21).ToList();
                            if(emO.Count != 1)
                            {
                                tmp.Name = "";
                                tmp.EmployeeNO = "";
                            }
                            else
                            {
                                tblEmployee emp2 = emp.Where(t => t.EmployeeNo.Replace(" ","") == emO.FirstOrDefault().EmployeeNo).FirstOrDefault();
                                if (emp2 != null)
                                {
                                    tmp.Name = emp2.EmployeeFirstName + " " + emp2.EmployeeLastName;
                                    tmp.EmployeeNO = emp2.EmployeeNo;
                                }
                                

                            }
                            
                        }
                        tmp.ApproveID = pID;
                        header.insertApproveStatus(tmp);
                       
                    }
                }
                tblEmployee empLog = emp.Where(t => t.EmployeeNo.Replace(" ","") == Data["EvaluatorNO"].ToString().Replace(" ","")).FirstOrDefault();
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
               // tblProjectMember proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).Where(a => a.StaffID == Data["EmployeeNO"].ToString()).FirstOrDefault();
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
            List< SP_GetEvaListByEvaluatorID_Result > evalist = header.getEvaListByEvaluatorID(EvaluatorID).Where(a=>a.EvaStatus!=3).OrderByDescending(a=>a.Eva_ID).ToList();
            List<JObject> Jeva = new List<JObject>();
            List<tblEmployee> emp = header.getEmployees();
            List<tblApprove> ap = header.GetAllApprove();
            List<tblProject> pr = header.getProject();
            evalist.ForEach(a =>
            {
            JObject tmp = new JObject();
            tblApprove t = ap.Where(x => x.EvaID == a.Eva_ID).FirstOrDefault();
            tmp["ApproveStat"] = (t != null) ? t.ApproveState : 0;
                if (a.ProjectCode != null)
                {
                    tblProject tt = pr.Where(x => x.ProjectCode.Trim() == ((a.ProjectCode.Contains('-') ? a.ProjectCode.Trim().Split('-')[1] : a.ProjectCode))).FirstOrDefault();
                    tmp["CustomerCompanyAlias"] = (a.ProjectCode.Contains('-') ? a.ProjectCode.Trim().Split('-')[0] : "");

                }
                tmp["EmployeeFirstName"] = a.EmployeeFirstName;
                tmp["EmployeeLastName"] = a.EmployeeLastName;
                tmp["EvaluatorFirstName"] = a.EvaluatorFirstName;
                tmp["EvaluatorLastName"] = a.EvaluatorLastName;
                tmp["EvaStatus"] = a.EvaStatus;
                tmp["EvaTerm"] = a.EvaTerm;
                tmp["Eva_ID"] = a.Eva_ID;
                tmp["Date"] = a.Date.ToString().Substring(0, 10) + " " + ((a.Date.ToString().ElementAt(11) == ':') ? '0' + a.Date.ToString().Substring(10, 4) : a.Date.ToString().Substring(10, 5));

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
                // tblEmployee empTemp = emp.Where(b => b.EmployeeNo.Replace(" ","") == a.em).FirstOrDefault();
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
            List<SP_GetEvaListByEvaluatorID_Result> evalist = header.getEvaListByEvaluatorID(EvaluatorID).Where(a => a.PeriodID == periodID).OrderByDescending(a => a.Eva_ID).ToList();
            List<JObject> Jeva = new List<JObject>();
            List<tblEmployee> emp = header.getEmployees();
            List<tblApprove> ap = header.GetAllApprove();
            evalist.ForEach(a =>
            {
                JObject tmp = new JObject();
                tblApprove t = ap.Where(x => x.EvaID == a.Eva_ID).FirstOrDefault();
                tmp["ApproveStat"] = (t != null) ? t.ApproveState : 0;
                if(a.ProjectCode !=null)
                tmp["CustomerCompanyAlias"] = (a.ProjectCode.Contains('-') ? a.ProjectCode.Trim().Split('-')[0] : "");
                tmp["EmployeeFirstName"] = a.EmployeeFirstName;
                tmp["EmployeeLastName"] = a.EmployeeLastName;
                tmp["EvaluatorFirstName"] = a.EvaluatorFirstName;
                tmp["EvaluatorLastName"] = a.EvaluatorLastName;
                tmp["EvaStatus"] = a.EvaStatus;
                tmp["EvaTerm"] = a.EvaTerm;
                tmp["Eva_ID"] = a.Eva_ID;
                tmp["Date"] = a.Date.ToString().Substring(0,10) + " " + ((a.Date.ToString().ElementAt(11) == ':')?'0'+ a.Date.ToString().Substring(10, 4): a.Date.ToString().Substring(10, 5));

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
                // tblEmployee empTemp = emp.Where(b => b.EmployeeNo.Replace(" ","") == a.em).FirstOrDefault();
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
            List<tblApproveStatus> ApS = header.GetApproveStatus().Where(a => a.EmployeeNO == EmpID).OrderByDescending(a=>a.ID).ToList();
            List<tblApprove> Ap = new List<tblApprove>();
            List<tblEvaluation> ListEva = header.GetAllEvaluation().ToList();
            List<tblFlowMaster> ListFlow = header.getAllFlow();
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
                List<tblApproveStatus> ApSt = ApS.Where(b => b.ApproveID == a.ID).OrderBy(b=>b.FlowOrder).ToList();
                tblEvaluation eva = ListEva.Where(b => b.Eva_ID == a.EvaID).FirstOrDefault();
                tmp["Date"] = eva.StartEvaDate.ToString().Substring(0, 9).Replace("-", "/") + " " + ((eva.StartEvaDate.ToString().ElementAt(11)==':')?'0'+ eva.StartEvaDate.ToString().Substring(10, 4).Replace("-", "/"): eva.StartEvaDate.ToString().Substring(10, 5).Replace("-", "/"));
                


                tblEmployee empTemp = emp.Where(b => b.EmployeeNo.Replace(" ","") == a.EmployeeNo).FirstOrDefault();
                tmp["name_language"] = JsonConvert.DeserializeObject < JObject > ("{\"EN\":\"" + empTemp.EmployeeFirstName + " " + empTemp.EmployeeLastName + "\",\"TH\":\"" + empTemp.EmployeeFirstNameThai + " " + empTemp.EmployeeLastNameThai + "\"}");
                ApObject.Add(tmp);
            });
            return ApObject;
        }

        [Route("Delete/{EvaID}")]
        [HttpDelete]
        public HttpResponseMessage DeleteData(int EvaID)
        {
            //อิอิอิอิอิอิอื
            try
            {
                var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
                tblEvaluation eva = header.GetAllEvaluation().Where(a => a.Eva_ID == EvaID).FirstOrDefault();
                header.DeleteEva(EvaID);
                header.DeleteApprove(EvaID);
                tblEmployee emp = header.getEmployees().Where(a => a.EmployeeNo.Replace(" ","") == eva.EvaluatorNO).FirstOrDefault();
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
                //tblEmployee em = emp.Where(b=>b.EmployeeNo.Replace(" ","") == a.)
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
            List<JObject> ApObject = new List<JObject>();
            List<tblApproveStatus> AllAps = header.GetApproveStatus();
            List<tblApproveStatus> ApS = AllAps.Where(a => ((a.EmployeeNO!=null)?a.EmployeeNO.Trim():a.EmployeeNO) == EmpID.Trim()).ToList();
            if (ApS != null)
            {
                List<tblEvaluation> ListEva = header.GetAllEvaluation();
                List<tblApprove> Ap = new List<tblApprove>();
                List<tblApprove> AllAp = header.GetAllApprove();
                foreach (tblApproveStatus aps in ApS.Where(a=>a.Status == 0).GroupBy(a => a.ApproveID).Select(a => a.First()).ToList())
                {
                    int num = 0;
                    List<tblApproveStatus> tmp = AllAps.Where(a => a.ApproveID == aps.ApproveID).ToList();
                    foreach (tblApproveStatus n in tmp)
                    {
                        if (n.ID == aps.ID)
                        {
                            break;
                        }
                        else if (n.Status == 0)
                        {
                            num = 1;
                        }
                    }

                    if (num == 0)
                    {
                        tblApprove Approve = AllAp.Where(a => a.ID == aps.ApproveID).FirstOrDefault();
                        if (Approve != null)
                        {
                            tblEvaluation EvaData = ListEva.Where(a => a.Eva_ID == Approve.EvaID).FirstOrDefault();
                            if (EvaData != null && EvaData.EvaStatus == 1)
                                Ap.Add(Approve);
                        }
                    }

                }
                List<tblEvaluation> evalist = header.GetAllEvaluation().ToList();
                List<tblEmployee> emp = header.getEmployees();
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
                    tblEvaluation eva = evalist.Where(b => b.Eva_ID == a.EvaID && b.EvaStatus == 1).FirstOrDefault();
                    List<tblApproveStatus> aps = header.GetApproveStatus().Where(x => x.ApproveID == a.ID).ToList();
                    tmp["Date"] = eva.StartEvaDate.ToString().Substring(0, 9).Replace("-", "/") + " " + ((eva.StartEvaDate.ToString().ElementAt(11) == ':') ? '0' + eva.StartEvaDate.ToString().Substring(10, 4).Replace("-", "/") : eva.StartEvaDate.ToString().Substring(10, 5));
                    aps.ForEach(x =>
                    {
                        if(x.ApproveDate != null)
                        {
                            tmp["Date"] = x.ApproveDate.ToString().Substring(0, 9).Replace("-", "/") + " " + ((x.ApproveDate.ToString().ElementAt(11) == ':') ? '0' + x.ApproveDate.ToString().Substring(10, 4).Replace("-", "/") : x.ApproveDate.ToString().Substring(10, 5));
                        }
                    });
                    
                    
                    tblEmployee empTemp = emp.Where(b => b.EmployeeNo.Replace(" ","") == a.EmployeeNo).FirstOrDefault();
                    tmp["name_language"] = JsonConvert.DeserializeObject<JObject>("{\"EN\":\"" + empTemp.EmployeeFirstName + " " + empTemp.EmployeeLastName + "\",\"TH\":\"" + empTemp.EmployeeFirstNameThai + " " + empTemp.EmployeeLastNameThai + "\"}");
                    ApObject.Add(tmp);
                });
                
            }

            return ApObject;

        }

        [Route("ApproveStatus")]
        [HttpPut]
        public void ApproveState([FromBody]JObject Data)
        {
            
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
           
            // var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            tblApprove AllAp = header.GetAllApprove().Where(a => a.EvaID == Convert.ToInt32(Data["EvaID"].ToString())).OrderByDescending(a=>a.ID).FirstOrDefault();
            tblApproveStatus ApS = header.GetApproveStatus().Where(a => ((a.EmployeeNO!=null)?a.EmployeeNO.Trim():a.EmployeeNO) == Data["EmpID"].ToString() && a.ApproveID == AllAp.ID).OrderByDescending(a=>a.ID).FirstOrDefault();
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
                //header.updateEvaluationStatus(Convert.ToInt32(Data["EvaID"].ToString()), 3);
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
                header.updateEvaluationStatus(Convert.ToInt32(Data["EvaID"].ToString()), 3);
            }
            if(Convert.ToInt32(Data["Status"].ToString()) == 0)
            {
                ApS.Status = -1;
            }
            header.UpdateApproveData(AllAp);
            header.UpdateApproveData(ApS);
            insertLog(AllAp.Name, Data["EmpID"].ToString(), type + " Approve EvaID : " + Data["EvaID"].ToString());
        }

        [Route("EvalistData")]
        [HttpGet]
        public JObject EvalistData()
        {

            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<object> result = new List<object>();
            List<tblEmployee> emp = header.getEmployees().Where(a=>a.QuitDate==null).ToList();
            List<tblPart2Master> Role = header.getPart2Data();
            List<tblProject> proj = header.getProject().OrderByDescending(a=>a.VersionNo).ToList();
           // var proj2 = header.getProject().GroupBy(a => a.ProjectCode).Select(grp => new {  LastProject = grp.OrderByDescending(x => x.VersionNo).FirstOrDefault() }).ToList();

            List<string> empname = new List<string>();
            List<JObject> Data = new List<JObject>();
            List<JObject> Em = new List<JObject>();
            string str = "";
            str = "{\"EN\":[";
            string t = "";
            emp.ForEach(n =>
            {
                if(n.EmployeeFirstName != null && n.EmployeeLastName != null)
                {
                    str += t + "\""+ n.EmployeeNo + " - " + n.EmployeeFirstName + " " + n.EmployeeLastName + "\"";
                    t = ",";
                }
                
            });
            str += "]}";
            JObject tmp2 = new JObject();
            tmp2["EN"] = JsonConvert.DeserializeObject<JObject>(str);
            t = "";
            str = "{\"TH\":[";
            emp.ForEach(n =>
            { 
                    str += t + "\"" + n.EmployeeFirstNameThai + " " + n.EmployeeLastNameThai + "\"";
                    t = ",";
            });
            str += "]}";
            tmp2["TH"] = JsonConvert.DeserializeObject<JObject>(str);
            str = "";
            str += "{\"Role\":[";
            
            t = "";
            Role.ForEach(n =>
            {

                str += t + "\"" + n.Function + "\"";
                t = ",";
            });
            str += "]}";
            tmp2["Role"] = JsonConvert.DeserializeObject<JObject>(str);
            str = "{\"Project\":[";
            t = "";
            List<tblProject> test = new List<tblProject>();
            proj.ForEach(n =>
            {
               if(test.Where(a=>a.ProjectNameAlias == n.ProjectNameAlias).ToList().Count == 0)
                {
                    str += t + "\"" +  n.ProjectNameAlias + "\"";
                    t = ",";
                    test.Add(n);
                }
                
            });
            str += "]}";
            tmp2["Project"] = JsonConvert.DeserializeObject<JObject>(str);

            // JObject a = JsonConvert.DeserializeObject<JObject>(emp);
            return tmp2;
        }


        [Route("ScoreDelete/{ScoreID}")]
        [HttpDelete]
        public void DeleteScore(int ScoreID)
        {

            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            header.DeleteScore(ScoreID);
        }

        [Route("ApproveFlow/{EvaID}")]
        [HttpGet]
        public List<JObject> ApproveFlow(int EvaID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            tblApprove app = header.getApprove().Where(a => a.EvaID == EvaID).OrderByDescending(a=>a.ID).FirstOrDefault();
            List<tblFlowMaster> FlowList = header.getAllFlow().ToList();
            List<tblApproveStatus> ApSList = header.GetApproveStatus().Where(a=>a.ApproveID == app.ID).ToList();
            List<JObject> Result = new List<JObject>();
            List<tblEmployee> emp2 = header.getEmployees().ToList();
            List<tblEmployeeOrganization> em = header.getEmployeeOrganization().Where(a=>a.PositionNo==21).ToList();
            ApSList.ForEach(a=> {
                JObject tmp = new JObject();
                tmp["Date"] = "";
                string str = "";
                tblEmployee emp = new tblEmployee();
                emp = null;
                if (a.EmployeeNO != null)
                {
                    emp = emp2.Where(b => b.EmployeeNo.Trim() == a.EmployeeNO.Trim()).FirstOrDefault();
                }
                
                str = "{\"EN\":\"\",\"TH\":\"\"}";
                if (emp != null)
                    str = "{\"EN\":\"" + emp.EmployeeFirstName + " " + emp.EmployeeLastName + "\",\"TH\":\"" + emp.EmployeeFirstNameThai + " " + emp.EmployeeLastNameThai + "\"}";

                if (a.Status == 1)
                {
                    tmp["Date"] = a.ApproveDate.ToString().Substring(0, 10).Replace("-", "/") + " " + ((a.ApproveDate.ToString().ElementAt(11) == ':')?'0'+a.ApproveDate.ToString().Substring(10,4):a.ApproveDate.ToString().Substring(10,5));
                    //((a.Date.ToString().ElementAt(11) == ':') ? '0' + a.Date.ToString().Substring(10, 4) : a.Date.ToString().Substring(10, 5))
                }
                tmp["EmployeeNo"] = (emp!=null)?a.EmployeeNO.Trim():null;
                tmp["Name"] = JsonConvert.DeserializeObject<JObject>(str);
                tmp["ProjectCode"] = app.ProjectCode;
                tmp["Role"] = FlowList[(int)a.FlowOrder-1].PositionName; ;
                tmp["Status"] = a.Status;
                Result.Add(tmp);
            });


            

            return Result;
        }

        [Route("GroupManager")]
        [HttpGet]
        public List<JObject> GroupManager()
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<tblEmployee> emp2 = header.getEmployees().Where(b => b.PositionNo == 21 && b.QuitDate == null).ToList();
            List<JObject> j = new List<JObject>();
          //  List<tblEmployee> tmpp = new List<tblEmployee>();
                List<tblEmployee> result = new List<tblEmployee>();
                List<tblEmployeeOrganization> t = header.getEmployeeOrganization().Where(b => b.PositionNo == 21).ToList();
                emp2.ForEach(c =>
                {
                    //if (result.Where(b => b.EmployeeNo.Trim() == c.EmployeeNo.Trim()).ToList().Count == 0 && result.Where(b => b.EmployeeFirstName.Trim() == c.EmployeeFirstName.Trim() && b.EmployeeLastName.Trim() == c.EmployeeLastName.Trim()).ToList().Count == 0)
                    {
                        result.Add(c);
                    }
                        
                });
                t.ForEach(c =>
                {
                    
                    tblEmployee e = emp2.Where(b => b.EmployeeNo.Trim() == c.EmployeeNo.Trim()).FirstOrDefault();
                    if(e!=null)
                    if ( result.Where(b => b.EmployeeNo.Trim() == c.EmployeeNo.Trim()).ToList().Count == 0 && result.Where(b => b.EmployeeFirstName.Trim() == e.EmployeeFirstName.Trim() && b.EmployeeLastName.Trim() == e.EmployeeLastName.Trim()).ToList().Count == 0)
                    {
                        result.Add(e);
                    }
                });
            result.Reverse();
            result.ForEach(a =>
            {
                JObject tmp = new JObject();
                string str = "{\"EN\":\"" + a.EmployeeFirstName + " " + a.EmployeeLastName + "\",\"TH\":\"" + a.EmployeeFirstNameThai + " " + a.EmployeeLastNameThai + "\"}";
                
                tmp["Name"] = JsonConvert.DeserializeObject<JObject>(str);
                tmp["EmployeeNo"] = a.EmployeeNo.Trim();
                j.Add(tmp);

            });
            return j;
           }
        [Route("UpdateGm")]
        [HttpPut]
        public void UpdateGM([FromBody]JObject Data)
        {

            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            tblApprove Ap = header.GetAllApprove().Where(a => a.EvaID == Convert.ToInt32(Data["EvaID"].ToString())).OrderByDescending(a=>a.ID).FirstOrDefault();
            tblApproveStatus Aps = header.GetApproveStatus().Where(a => a.ApproveID == Ap.ID && a.FlowOrder == 3).FirstOrDefault();
            tblEmployee emp = header.getEmployees().Where(a => a.EmployeeNo.Trim() == Data["EmployeeNo"].ToString().Trim()).FirstOrDefault();
            header.UpdateGM(Aps.ID, emp.EmployeeNo.Trim(), emp.EmployeeFirstName + " " + emp.EmployeeLastName);
        }

        }
}
