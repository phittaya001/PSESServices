using CSI.CastleWindsorHelper;
using Newtonsoft.Json.Linq;
using PesWeb.Service;
using PesWeb.Service.Modules;
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

        [Route("All/{PositionID}/{EvaID}")]
        [HttpGet]
        public List<SP_GetHeaderByPosition_Result> GetAllHeader(int PositionID,int EvaID)
        {

            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            var header2 = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<tblScore> sc = header2.GetAllScore();
            List<tblHeader> hd = header.GetAllHeader().ToList();
            List<tblHeaderJob> Allhj = header.getAllHeaderJob().ToList();
            // tblProjectMember proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).Where(a => a.StaffID == Data["EmployeeNO"].ToString()).FirstOrDefault();
            SP_GetEvaDataByEvaID_Result eva = header2.getEvaDataByEvaID(EvaID).Where(a=>a.Part2ID == PositionID).FirstOrDefault();
            List<tblHeaderJob> hj = header.getAllHeaderJob().Where(a => a.PositionNo == eva.Part2ID).ToList();
            List<SP_GetHeaderByPosition_Result> GetHeader = header.getHeaderByPosition(PositionID, EvaID).ToList();
            List<tblHeader> Ans = new List<tblHeader>();

            if (GetHeader.Count == GetHeader.Where(a=>a.point==null).ToList().Count)
                foreach (tblHeaderJob tmp in hj)
            {
                foreach (tblHeader hd2 in hd.Where(a => a.H_ID == tmp.H1_ID))
                {
                    
                        foreach (tblHeader hd3 in FinalHeader(hd2, hd))
                    {
                        if (sc.Where(a => a.Eva_ID == EvaID && a.H3_ID == hd3.H_ID).ToList().Count==0)
                            if (Ans.Where(a => a.H_ID == hd3.H_ID).ToList().Count == 0)
                            Ans.Add(hd3);
                    }
                }
            }
            foreach (tblHeader h in Ans)
            {
                header2.InsertSCORE(EvaID, h.H_ID);
            }

            GetHeader = header.getHeaderByPosition(PositionID, EvaID).ToList();
            List<SP_GetHeaderByPosition_Result> H = new List<SP_GetHeaderByPosition_Result>();
            List<SP_GetHeaderByPosition_Result> H2 = new List<SP_GetHeaderByPosition_Result>();
            // GetHeader.Reverse(0,GetHeader.Count);

            List<tblHeaderAdditional> HdA = header.getHeaderAdditional().Where(a => a.Eva_ID == EvaID).ToList();
            foreach(tblHeaderAdditional HdATemp in HdA)
            {
                SP_GetHeaderByPosition_Result newHeader = new SP_GetHeaderByPosition_Result();
                newHeader.Alias = HdATemp.Alias;
                newHeader.H_Level = HdATemp.H_Level;
                newHeader.Parent = HdATemp.parent;
                newHeader.Text = HdATemp.Text;
                newHeader.Text_Eng = HdATemp.Text_Eng;
                newHeader.Eva_ID = HdATemp.Eva_ID;
                newHeader.H_ID = (-1)*HdATemp.H_ID;
                newHeader.PositionNO = PositionID;
                newHeader.point = HdATemp.point;
                GetHeader.Add(newHeader);
            }

            foreach (SP_GetHeaderByPosition_Result a in GetHeader)
            {
                if (a.Parent == 0)
                {
                    H.Add(a);
                }
                else
                {
                    int parent = (int)a.Parent;
                    for(int i = 0; i < H.Count; i++)
                    {
                        if (H[i].H_ID == parent)
                        {
                            H.Insert(i+1, a);
                        }
                    }
                }
            }
            List<SP_GetHeaderByPosition_Result> H_new = new List<SP_GetHeaderByPosition_Result>();
            foreach (SP_GetHeaderByPosition_Result a in H)
            {
                if (a.Parent == 0)
                {
                    H_new.Add(a);
                }
                else
                {
                    int parent = (int)a.Parent;
                    for (int i = 0; i < H_new.Count; i++)
                    {
                        if (H_new[i].H_ID == parent)
                        {
                            H_new.Insert(i + 1, a);
                        }
                    }
                }
            }
            return H_new;
        }


        [Route("HeaderTop/Job/{JobID}")] // 
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
        public List<tblPart2Master> getJobDetail()
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
        public HttpResponseMessage InsertHeaderTop([FromBody]JObject Data)
        {
            try
            {
                var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
                //tblHeaderTop top = new tblHeaderTop();
                //top.Alias = Data["Alias"].ToString();
                //top.Text = Data["Text"].ToString();
                //top.Text_Eng = Data["Text_Eng"].ToString();

                header.InsertHeaderTop(Data["Text"].ToString(), Data["Text_Eng"].ToString(), Data["Alias"].ToString(), Convert.ToInt32(Data["PositionId"].ToString()));
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
            
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

        /////////////////////////////////////////new Tale

        [Route("Level/{HeaderLevel}")]
        [HttpGet]
        public List<tblHeaderLevel> GetHeaderByLevel(int HeaderLevel)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            return header.getHeaderLevel().Where(a => a.CurrentLevel == HeaderLevel).ToList();

        }

        [Route("Insert")]
        [HttpPut]
        public void InsertAdditionalHeader([FromBody]JObject Data)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            tblHeaderAdditional H = new tblHeaderAdditional();
            int positionNo = Convert.ToInt32(Data["PositionNo"].ToString());
            H.Eva_ID = Convert.ToInt32(Data["Eva_Id"].ToString());
            
            H.parent = Convert.ToInt32(Data["H_ID"].ToString());
            //tblHeader hd = header.GetAllHeader().Where(a => a.H_ID == H.parent).FirstOrDefault();
            H.H_Level = (Convert.ToInt32(Data["H_ID"].ToString())>0)? header.GetAllHeader().Where(a => a.H_ID == H.parent).FirstOrDefault().H_Level + 1 : header.getHeaderAdditional().Where(a => a.H_ID == (-1)*H.parent).FirstOrDefault().H_Level+1;
            H.Text = Data["Text"].ToString(); 
            H.Text_Eng = Data["Text_Eng"].ToString(); 
            H.Alias = Data["Alias"].ToString();
            H.point = 0;
            header.InsertAdditionalHeader(H);
        }

    }
}
