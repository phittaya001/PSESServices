using Newtonsoft.Json.Linq;
using PesWeb.Service;
using PesWeb.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PesWeb.Injectors;
using CSI.CastleWindsorHelper;

namespace PESproj.Controllers
{
    [RoutePrefix("Form")]
    public class FormController : ApiController
    {
        [Route("CreateEvaHeader")]
        [HttpPost]
        public string createForm([FromBody]JObject Data)
        {
            tblEvaluation Eva = new tblEvaluation();
            Eva.EmployeeNO = Data["EmployeeNO"].ToString();
            Eva.EvaluatorNO = Data["EvaluatorNO"].ToString();
            Eva.Job_ID = Convert.ToInt32(Data["JobID"].ToString());
            Eva.ProjectNO = Data["ProjectNO"].ToString();
            var svc = ServiceContainer.GetService<PesWeb.Service.Modules.FormManage>();

            PSESEntities db = new PSESEntities();
            List<SP_GetAllHeaderByJobID_Result> AllHeader = db.SP_GetAllHeaderByJobID().Where(a => a.JobID == Convert.ToInt32(Data["JobID"].ToString())).ToList();

            int EvaID = svc.createForm(Eva);
            tblScore score = new tblScore();
            foreach(SP_GetAllHeaderByJobID_Result temp in AllHeader)
            {
                score.Eva_ID = EvaID;
                score.H3_ID = temp.H3_ID;
                svc.InsertScore(score);
            }
            return "Success";
        }

        [Route("EvaHeader/{EvaID}")]
        [HttpGet]
        public List<SP_GetEvaHeaderByEvaID_Result> GetEvaHeader(int EvaID)
        {
            var svc = ServiceContainer.GetService<PesWeb.Service.Modules.FormManage>();
            
            return svc.getEvaDataByEvaID(EvaID).ToList();
        }

    }
}
