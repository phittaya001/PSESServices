using CSI.CastleWindsorHelper;
using Newtonsoft.Json.Linq;
using PesWeb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PESproj.Controllers
{
    [RoutePrefix("Header")]
    public class HeaderController : ApiController
    {
        [Route("HeaderTop")]
        [HttpGet]
        public List<tblHeaderTop> GetAllHeaderTop()
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeaderTop> GetHeaderTop = header.getAllHeaderTop().ToList();
            return GetHeaderTop;
        }

        [Route("HeaderTop/Job/{JobID}")]
        [HttpGet]
        public List<SP_HeaderTopByJobID_Result> GetHeaderTopByJobID(int JobID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<SP_HeaderTopByJobID_Result> GetHeaderTop = header.getHeaderByJob().Where(a=>a.PositionNo==JobID).ToList();
            return GetHeaderTop;
        }

        [Route("HeaderTop/HJ/{HJID}")]
        [HttpGet]
        public List<tblHeaderTop> GetHeaderTopByHJID(int HJID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeaderTop> GetHeaderTop = header.getAllHeaderTop().ToList();
            return GetHeaderTop;
        }

        [Route("AllHeader")]
        [HttpGet]
        public List<SP_GetAllHeaderByJobID_Result> GetAllHeader()
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<SP_GetAllHeaderByJobID_Result> AllHeader = header.getAllHeader();
            return AllHeader;
        }

        [Route("AllHeader/{JobID}")]
        [HttpGet]
        public List<SP_GetAllHeaderByJobID_Result> GetAllHeaderByJobID(int JobID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<SP_GetAllHeaderByJobID_Result> AllHeader = header.getAllHeader().Where(a=>a.JobID==JobID).ToList();
            return AllHeader;
        }

        [Route("position")]
        [HttpGet]
        public List<tblPosition> getJobDetail()
        {
            var svc = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            return svc.getJobDetail();
        }

        [Route("HeaderJob")]
        [HttpGet]
        public List<tblHeaderJob> getAllHeaderJob()
        {
            var svc = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            return svc.getAllHeaderJob();
        }

        [Route("HeaderJob/{JobID}")]
        [HttpGet]
        public List<tblHeaderJob> GetHederJobByJobID(int jobID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeaderJob> HederJobByJobID = header.getAllHeaderJob().Where(a => a.PositionNo == jobID).ToList();
            return HederJobByJobID;
        }

        [Route("HeaderMid")]
        [HttpGet]
        public List<tblHeaderMid> GetAllHeaderMid()
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeaderMid> GetHeaderMid = header.getAllHeaderMid();
            return GetHeaderMid;
        }

        [Route("HeaderMid/ByHJ/{HeaderJobID}")]
        [HttpGet]
        public List<tblHeaderMid> GetHeaderMidByHeaderJobID(int HeaderjobID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeaderMid> HeadrMidByHearderJobID = header.getAllHeaderMid().Where(a => a.HJ_ID == HeaderjobID).ToList();
            return HeadrMidByHearderJobID;
        }

        [Route("HeaderMid/{HeaderTopID}/{JobID}")]
        [HttpGet]
        public List<SP_GetHeaderMidByHeaderTopAndJobID_Result> GetHeaderMidByHeaderTopIDAndJobID(int HeaderTopID,int JobID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<SP_GetHeaderMidByHeaderTopAndJobID_Result> HeadrMidByHearderTobIDAndJobID = header.GetHeaderMidByHeaderTopAndJobID(HeaderTopID, JobID).ToList();
            return HeadrMidByHearderTobIDAndJobID;
        }

        [Route("HeaderBot")]
        [HttpGet]
        public List<tblHeaderBot> GetAllHeaderBot()
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeaderBot> GetHeaderBot = header.getAllHeaderBot();
            return GetHeaderBot;
        }

        [Route("HeaderBot/{HeaderMidID}")]
        [HttpGet]
        public List<tblHeaderBot> GetHeaderBotByHeaderMidID(int HeaderMidID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeaderBot> GetHeaderBot = header.getAllHeaderBot().Where(a=>a.H2_ID== HeaderMidID).ToList();
            return GetHeaderBot;
        }

        [Route("HeaderTop/Delete/{HeaderTopID}/{JobID}")]
        [HttpDelete]
        public List<tblHeaderTop> DeleteHeaderTop(int HeaderTopID,int JobID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            header.DeleteHeaderTop(HeaderTopID, JobID);
            return header.getAllHeaderTop().ToList();
        }

        [Route("HeaderMid/Delete/{HeaderMidID}")]
        [HttpDelete]
        public void DeleteHeaderMid(int HeaderMidID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            header.DeleteHeaderMid(HeaderMidID);
        }

        [Route("HeaderBot/Delete/{HeaderBotID}")]
        [HttpDelete]
        public void DeleteHeaderBot(int HeaderBotID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            header.DeleteHeaderBot(HeaderBotID);
        }

        [Route("HeaderTop/Insert")]
        [HttpPut]
        public void InsertHeaderTop([FromBody]JObject Data)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            header.InsertHeaderTop(Data["Text"].ToString(), Data["Alias"].ToString(), Data["Text_Eng"].ToString(),Convert.ToInt32(Data["JobID"].ToString()));
        }

        [Route("HeaderMid/Insert")]
        [HttpPut]
        public void InsertHeaderMid([FromBody]JObject Data)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            header.InsertHeaderMid(Data["Text"].ToString(), Data["Text_Eng"].ToString(), Convert.ToInt32(Data["H1_ID"].ToString()), Convert.ToInt32(Data["JobID"].ToString()));
        }

        [Route("HeaderBot/Insert")]
        [HttpPut]
        public void InsertHeaderBot([FromBody]JObject Data)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            header.InsertHeaderBot(Data["Text"].ToString(), Convert.ToInt32(Data["H2_ID"].ToString()));
        }
    }
}
