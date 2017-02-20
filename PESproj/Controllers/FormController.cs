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
        public int createForm([FromBody]JObject Data)
        {
            tblEvaluation Eva = new tblEvaluation();
            Eva.EmployeeNO = Data["EmployeeNO"].ToString();
            Eva.EvaluatorNO = Data["EvaluatorNO"].ToString();
            Eva.Job_ID = Convert.ToInt32(Data["JobID"].ToString());
            Eva.ProjectNO = Data["ProjectNO"].ToString();
            var svc = ServiceContainer.GetService<PesWeb.Service.Modules.FormManage>();
            svc.createForm(Eva);

            return svc.getEvaID(Eva);
        }

        [Route("CreateDataByEvaID/{EvaID}")]
        [HttpGet]
        public int Evaluation(int EvaID)
        {

            return 0;
        }
    }
}
