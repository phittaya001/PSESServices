using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PesWeb.Service;
using CSI.CastleWindsorHelper;
using Newtonsoft.Json.Linq;
using PESproj.Views;
using PESproj.Views.Control;

namespace PESproj.Controllers
{
    [RoutePrefix("Report")]
    public class ReportController : ApiController
    {
        [Route("Name")]
        [HttpGet]
        public List<tblReport> getReport()
        {
            //tblReport a = new tblReport();
            var report = ServiceContainer.GetService<PesWeb.Service.Modules.ReportManage>();
            return report.getAllReport();
        }

        [Route("Group/{Type}")]
        [HttpGet]
        public List<string> getGroup(string Type)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
           // 
            List<tblOrganization> result = new List<tblOrganization>();
            List<string> str = new List<string>();
            List<tblOrganization> all = header.getOrganization();
            if (Type == "nonsub")
            {
                //all.ForEach(a =>
                //{
                //    int p = a.OrganizationAlias.IndexOf("-");
                //    if (p < 0)
                //    {
                //        result.Add(a);
                //    }
                //});
                result = all.Where(a => a.OrganizationAlias.Contains("-") == false).ToList();

            }
            else if(Type == "all")
            {
                result = all.Where(a => a.OrganizationAlias.Contains("-") == true).ToList();
            }
            else
            {
                result = all.Where(a => a.OrganizationAlias.Contains(Type) == true && a.OrganizationAlias != Type).ToList();
            }
            
            result.ForEach(a =>
            {
                List<string> tmp = a.OrganizationAlias.Split('-').ToList();
                if (Type != "nonsub")
                    str.Add(tmp[1]);
                else
                    str.Add(tmp[0]);
            });
            return str;
        }
        
        [Route("Report1/{Group}/{SubGroup}/{PeriodID}")]
        [HttpGet]
        public List<tblEvaluation> Report1(string Group,string SubGroup,int PeriodID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            int groupn = 0;
            int evatmp = 0;
            if(Group == "0")
            {
                //tblOrganization g = header.getOrganization().Where(a => a.OrganizationAlias == Group).FirstOrDefault();
                //if(g!=null)
                //groupn = g.OrganizationNo;
            }
            if(SubGroup!= "0" && Group != "0")
            {
                tblOrganization g = header.getOrganization().Where(a => a.OrganizationAlias == Group + "-" + SubGroup).FirstOrDefault();
                if(g!=null)
                groupn = g.OrganizationNo;
                else
                {
                    groupn = -1;
                }
            }
            List<tblEmployee> listem = header.getEmployees();

            //tblReport a = new tblReport();
            
            var e = header.GetAllEvaluation().OrderBy(a=>a.EvaluatorNO).ToList();
            List<tblEmployeeOrganization> emo = header.getEmployeeOrganization();
            List<tblEvaluation> eva = new List<tblEvaluation>();
            List<tblOrganization> or = header.getOrganization();
            if(PeriodID == 0 )
            {
                if(SubGroup == "0" && Group == "0")
                {
                    List<tblEvaluation> result = new List<tblEvaluation>();
                    e.ForEach(a =>
                    {
                        //if(evatmp )
                        tblEvaluation tmp = new tblEvaluation();
                        tmp.EvaluatorNO = a.EvaluatorNO;
                        tmp.EmployeeNO = a.EmployeeNO;
                        tmp.Eva_ID = a.Eva_ID;
                        tmp.PeriodID = a.PeriodID;
                        result.Add(tmp);
                    });
                    return result;
                }
                else
                {
                    if(SubGroup != "0")
                    {
                        List<tblEvaluation> result = new List<tblEvaluation>();
                        e.ForEach(a =>
                        {
                            if (emo.Where(x => x.EmployeeNo.Trim() == a.EmployeeNO.Trim() && x.OrganizationNo == groupn).ToList().Count > 0)
                            {
                                tblEvaluation tmp = new tblEvaluation();
                                tmp.EvaluatorNO = a.EvaluatorNO;
                                tmp.EmployeeNO = a.EmployeeNO;
                                tmp.Eva_ID = a.Eva_ID;
                                tmp.PeriodID = a.PeriodID;
                                result.Add(tmp);
                            }

                        });
                        return result;
                    }else
                    {
                        List<tblEvaluation> result = new List<tblEvaluation>();
                        e.ForEach(a =>
                        {
                            List<tblOrganization> ortmp = or.Where(x => x.OrganizationAlias.Contains(Group) == true).ToList();
                            ortmp.ForEach(y =>
                            {
                                if (emo.Where(x => x.EmployeeNo.Trim() == a.EmployeeNO.Trim() && x.OrganizationNo == y.OrganizationNo).ToList().Count > 0 && result.Where(x=>x.Eva_ID==a.Eva_ID).ToList().Count==0)
                                {
                                    tblEvaluation tmp = new tblEvaluation();
                                    tmp.EvaluatorNO = a.EvaluatorNO;
                                    tmp.EmployeeNO = a.EmployeeNO;
                                    tmp.Eva_ID = a.Eva_ID;
                                    tmp.PeriodID = a.PeriodID;
                                    result.Add(tmp);
                                }
                            });
                        });
                        return result;

                    }

                }
                
            }
            else
            {
                
                if (SubGroup == "0" && Group == "0")
                {
                    List<tblEvaluation> result = new List<tblEvaluation>();
                    e.ForEach(a =>
                    {
                        if(a.PeriodID == PeriodID){
                            tblEvaluation tmp = new tblEvaluation();
                            tmp.EvaluatorNO = a.EvaluatorNO;
                            tmp.EmployeeNO = a.EmployeeNO;
                            tmp.Eva_ID = a.Eva_ID;
                            tmp.PeriodID = a.PeriodID;
                            result.Add(tmp);
                        }
                        
                    });
                    return result;
                }
                else
                {
                    if (SubGroup != "0")
                    {
                        List<tblEvaluation> result = new List<tblEvaluation>();
                        e.ForEach(a =>
                        {
                            if (emo.Where(x => x.EmployeeNo.Trim() == a.EmployeeNO.Trim() && x.OrganizationNo == groupn).ToList().Count > 0)
                            {
                                if (a.PeriodID == PeriodID)
                                {
                                    tblEvaluation tmp = new tblEvaluation();
                                    tmp.EvaluatorNO = a.EvaluatorNO;
                                    tmp.EmployeeNO = a.EmployeeNO;
                                    tmp.Eva_ID = a.Eva_ID;
                                    tmp.PeriodID = a.PeriodID;
                                    result.Add(tmp);
                                }

                            }

                        });
                        return result;
                    }
                    else{
                        List<tblEvaluation> result = new List<tblEvaluation>();
                        e.ForEach(a =>
                        {
                            List<tblOrganization> ortmp = or.Where(x => x.OrganizationAlias.Contains(Group) == true).ToList();
                            ortmp.ForEach(y =>
                            {
                                if (emo.Where(x => x.EmployeeNo.Trim() == a.EmployeeNO.Trim() && x.OrganizationNo == y.OrganizationNo).ToList().Count > 0)
                                {
                                    if (a.PeriodID == PeriodID)
                                    {
                                        tblEvaluation tmp = new tblEvaluation();
                                        tmp.EvaluatorNO = a.EvaluatorNO;
                                        tmp.EmployeeNO = a.EmployeeNO;
                                        tmp.Eva_ID = a.Eva_ID;
                                        tmp.PeriodID = a.PeriodID;
                                        result.Add(tmp);
                                    }
                                }
                            });
                        });
                        return result;
                    }
                    
                }
            }
            //return header.GetAllEvaluation();
        }

        [Route("Test")]
        [HttpGet]
        public void test()
        {
            Handler1 a = new Handler1();
           // System.Web.HttpContext b = new System.Web.HttpContext(new System.Web.HttpRequest("", "www.google.com", ""), new System.Web.HttpResponse(new System.IO.StringWriter()));
             
            a.ProcessRequest(System.Web.HttpContext.Current);
        }

    }
}
