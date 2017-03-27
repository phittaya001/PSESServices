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

        [Route("Delete/{H_ID}")]
        [HttpDelete]
        public void DeleteHeader(int H_ID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();

            header.DeleteHeader(H_ID);
        }

        [Route("All/{PositionID}/{EvaID}/{ID}")]
        [HttpGet]
        public List<SP_GetHeaderByPosition_Result> GetAllHeader(int PositionID,int EvaID,int ID)
        {

            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            var header2 = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            List<tblScore> sc = header2.GetAllScore();
            List<tblHeader> hd = header.GetAllHeader().ToList();
            //List<tblHeaderJob> Allhj = header.getAllHeaderJob().ToList();
            // tblProjectMember proj = header.getProjectMember().Where(a => a.ProjectID == Data["ProjectNO"].ToString()).Where(a => a.StaffID == Data["EmployeeNO"].ToString()).FirstOrDefault();
            SP_GetEvaDataByEvaID_Result eva = header2.getEvaDataByEvaID(EvaID).Where(a=>a.Part2ID == PositionID).FirstOrDefault();
            List<tblHeaderJob> hj = header.getAllHeaderJob().Where(a => a.PositionNo == ((eva!=null)? eva.Part2ID: PositionID)).ToList();
            List<SP_GetHeaderByPosition_Result> GetHeader = header.getHeaderByPosition(PositionID, EvaID).OrderBy(a=>a.H_ID).ToList();
            List<tblHeader> Ans = new List<tblHeader>();
            tblHeader p = new tblHeader();
            int sum = 0;
            foreach(tblHeaderJob hh in hj)
            {
                if(hd.Where(a => a.H_ID == hh.H1_ID).FirstOrDefault() != null)
                sum += FinalHeader(hd.Where(a=>a.H_ID==hh.H1_ID).FirstOrDefault() , hd).ToList().Count;
            }
            p.H_ID = 0;
            int a1 = GetHeader.Where(a => a.Eva_ID == EvaID).ToList().Count;
            int a2 = GetHeader.Where(a => a.Score_ID > 0).Where(a => a.Eva_ID == EvaID).ToList().Count;
            if (a2 < sum)
                foreach (tblHeaderJob tmpHJ in hj)
                {
                    foreach (tblHeader hd2 in hd.Where(a => a.H_ID == tmpHJ.H1_ID))
                    {

                        foreach (tblHeader hd3 in FinalHeader(hd2, hd))
                        {
                            if (sc.Where(a => a.Eva_ID == EvaID && a.H3_ID == hd3.H_ID).ToList().Count == 0)
                                if (Ans.Where(a => a.H_ID == hd3.H_ID).ToList().Count == 0)
                                    Ans.Add(hd3);
                        }
                    }
                }
            foreach (tblHeader h in Ans)
            {
                header2.InsertSCORE(EvaID, h.H_ID);
            }

            GetHeader = header.getHeaderByPosition(PositionID, EvaID).OrderBy(a=>a.H_ID).ToList();
            List<SP_GetHeaderByPosition_Result> H = new List<SP_GetHeaderByPosition_Result>();
            //List<SP_GetHeaderByPosition_Result> H2 = new List<SP_GetHeaderByPosition_Result>();
            // GetHeader.Reverse(0,GetHeader.Count);
            List<SP_GetHeaderByPosition_Result> GetHeader2 = new List<SP_GetHeaderByPosition_Result>();
            List<tblHeaderAdditional> HdA = header.getHeaderAdditional().Where(a => a.Eva_ID == EvaID && a.Part2ID == PositionID).ToList();
            foreach(tblHeaderAdditional HdATemp in HdA)
            {
                if (HdATemp.H_status == 1 || ID == 2)
                {
                    SP_GetHeaderByPosition_Result newHeader = new SP_GetHeaderByPosition_Result();
                    newHeader.Alias = HdATemp.Alias;
                    newHeader.H_Level = HdATemp.H_Level;
                    newHeader.Parent = HdATemp.parent;
                    newHeader.Text = HdATemp.Text;
                    newHeader.Text_Eng = HdATemp.Text_Eng;
                    newHeader.Eva_ID = HdATemp.Eva_ID;
                    newHeader.H_ID = (-1) * HdATemp.H_ID;
                    newHeader.PositionNO = PositionID;
                    newHeader.point = HdATemp.point;
                    //if (HdATemp.point ==null && HdATemp.point != 0)
                    //{
                    //    header.UpdateScoreData(EvaID, 0, (-1)*HdATemp.H_ID);
                    //    newHeader.point = 0;
                    //}
                    
                    GetHeader.Add(newHeader);
                }
                else if(ID == 1)
                {
                    DeleteHeader((-1)*HdATemp.H_ID);
                }
               
            }
            int j = 1;
            List<SP_GetHeaderByPosition_Result> H_new = new List<SP_GetHeaderByPosition_Result>();
            List<SP_GetHeaderByPosition_Result> hder = GetHeader.Where(a => a.H_ID > 0).OrderBy(a => a.H_ID).ToList();
            foreach (SP_GetHeaderByPosition_Result a in hder )
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

            foreach (SP_GetHeaderByPosition_Result a in GetHeader.Where(a => a.H_ID < 0))
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

            List<SP_GetHeaderByPosition_Result> H_new2 = new List<SP_GetHeaderByPosition_Result>();
            foreach (SP_GetHeaderByPosition_Result a in H_new)
            {
                if (a.Parent == 0)
                {
                    H_new2.Add(a);
                }
                else
                {
                    int parent = (int)a.Parent;
                    for (int i = 0; i < H_new2.Count; i++)
                    {
                        if (H_new2[i].H_ID == parent)
                        {
                            H_new2.Insert(i + 1, a);
                        }
                    }
                }
            }
            

            return H_new2;
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
           H.Part2ID = Convert.ToInt32(Data["PositionNo"].ToString());
            H.Eva_ID = Convert.ToInt32(Data["Eva_Id"].ToString());
            
            H.parent = Convert.ToInt32(Data["H_ID"].ToString());
            //tblHeader hd = header.GetAllHeader().Where(a => a.H_ID == H.parent).FirstOrDefault();
            H.H_Level = (Convert.ToInt32(Data["H_ID"].ToString()) == 0)? 1: (Convert.ToInt32(Data["H_ID"].ToString())>0)? header.GetAllHeader().Where(a => a.H_ID == H.parent).FirstOrDefault().H_Level + 1 : header.getHeaderAdditional().Where(a => a.H_ID == (-1)*H.parent).FirstOrDefault().H_Level+1;
            H.Text = Data["Text"].ToString(); 
            H.Text_Eng = Data["Text_Eng"].ToString(); 
            H.Alias = Data["Alias"].ToString();
            H.point = 0;

            header.InsertAdditionalHeader(H);
        }


        [Route("Average/")]
        [HttpPost]
        public List<SP_GetHeaderByPosition_Result> AvgScore([FromBody]List<JObject> Data)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<SP_GetHeaderByPosition_Result> sc = new List<SP_GetHeaderByPosition_Result>();
            int i = 0;
            foreach (JObject j in Data)
            {
                SP_GetHeaderByPosition_Result c = new SP_GetHeaderByPosition_Result();
                c.H_ID = Convert.ToInt32(j["H_ID"].ToString());
                c.point = Convert.ToInt32(j["point"].ToString());
                c.Parent = Convert.ToInt32(j["Parent"].ToString());
                //c.Score_ID = Convert.ToInt32(j["Score_ID"].ToString());
                c.Eva_ID = i;
                i++;
                sc.Add(c);
            }
            foreach (SP_GetHeaderByPosition_Result g in sc)
            {

                if (sc.Where(a => a.Parent == g.H_ID).ToList().Count > 0)
                {
                    List<SP_GetHeaderByPosition_Result> tmp = sc.Where(a => a.Parent == g.H_ID).ToList();
                    int sum = 0;
                    foreach (SP_GetHeaderByPosition_Result tmp2 in tmp)
                    {
                        if (tmp2.point != null)
                            sum += (int)tmp2.point;
                    }
                    sc[sc.IndexOf(g)].point = sum / tmp.Count;
                }
            }

            return sc;
        }

        public List<tblHeaderAdditional> FinalHeader(tblHeaderAdditional parent, List<tblHeaderAdditional> ListAll)
        {
            List<tblHeaderAdditional> ListResult = new List<tblHeaderAdditional>();
            if (ListAll.Where(a => a.parent == parent.H_ID * (-1)).ToList().Count == 0)
            {
                ListResult.Add(parent);
                return ListResult;
            }
            List<tblHeaderAdditional> Result = new List<tblHeaderAdditional>();
            foreach (tblHeaderAdditional res in ListAll.Where(a => a.parent == parent.H_ID * (-1)).ToList())
            {
                foreach (tblHeaderAdditional a in FinalHeader(res, ListAll))
                {
                    Result.Insert(Result.Count, a);
                    
                }
                    
            }
            return Result;
        }

        [Route("Update")]
        [HttpPut]
        public void UpdateHeader([FromBody]List<JObject> Data)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            var header2 = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            int EvaID = 0;
            foreach(JObject jo in Data)
            {
                if (jo.Count>2 && jo["Score"].ToString() == "N/A"){
                    header.UpdateScoreData(Convert.ToInt32(jo["EvaId"].ToString()), 0, Convert.ToInt32(jo["Id"].ToString()),string.IsNullOrEmpty(jo["Comment"].ToString())? "": jo["Comment"].ToString());
                }
                else
                {
                    header.UpdateScoreData(Convert.ToInt32(jo["EvaId"].ToString()), Convert.ToInt32(jo["Score"].ToString()), Convert.ToInt32(jo["Id"].ToString()),"");
                }
                EvaID = Convert.ToInt32(jo["EvaId"].ToString());
            }
            int sum = 0, sum2 = 0;
            List<tblHeaderAdditional> hAd = header.getHeaderAdditional().ToList();
            List<tblScore> score = header2.GetAllScore().Where(a => a.Eva_ID == EvaID).OrderBy(a=>a.H3_ID).ToList();
            List<tblHeader> hd = header.GetAllHeader().ToList();
            tblEvaluation eva = header2.GetAllEvaluation().Where(a => a.Eva_ID == EvaID).FirstOrDefault();
            List<tblHeaderJob> hj = header.getAllHeaderJob().Where(a => a.PositionNo == eva.Job_ID).ToList();
            foreach(tblHeaderJob th in hj)
            {
                if(hd.Where(a=>a.H_ID == th.H1_ID).ToList().Count > 0)
                {
                    List<tblHeader> H1 = hd.Where(a => a.Parent == th.H1_ID).ToList();
                    foreach (tblHeader th2 in H1)
                    {
                        List<tblHeader> H2 = hd.Where(a => a.Parent == th2.H_ID).ToList();
                        foreach (tblHeader th3 in H2)
                        {

                            sum += (int)score.Where(a => a.H3_ID == th3.H_ID).FirstOrDefault().point;
                        }
                        List<tblHeaderAdditional> Hd1 = hAd.Where(a => a.parent == th2.H_ID).ToList();
                        foreach (tblHeaderAdditional thA in Hd1)
                        {
                            sum += (int)thA.point;
                        }

                        header.UpdateScoreData(EvaID, sum / H2.Count, th2.H_ID,score.Where(a=>a.Eva_ID == EvaID && a.H3_ID == th2.H_ID).FirstOrDefault().Comment);
                        sum2 += sum / H2.Count;
                        sum = 0;
                    }
                    header.UpdateScoreData(EvaID, sum2 / H1.Count, (int)th.H1_ID, score.Where(a => a.Eva_ID == EvaID && a.H3_ID == th.H1_ID).FirstOrDefault().Comment);
                    sum2 = 0;
                }
                
            }
                
            





            sum = 0; sum2 = 0;
            List<tblHeaderAdditional> hdaList = header.getHeaderAdditional().Where(a=>a.Eva_ID==EvaID).ToList();
            List<tblHeaderAdditional> hh = hdaList.Where(a => a.parent == 0 && a.Eva_ID == EvaID).ToList();
            foreach (tblHeaderAdditional tmp in hh )
            {
                List<tblHeaderAdditional> h1 = hdaList.Where(a => a.parent == (-1) * tmp.H_ID).ToList();
                foreach (tblHeaderAdditional tmp2 in h1)
                {
                    List<tblHeaderAdditional> h2 = hdaList.Where(a => a.parent == (-1) * tmp2.H_ID).ToList();
                    foreach (tblHeaderAdditional tmp3 in h2)
                    {
                        sum += (int)tmp3.point;
                    }
                    sum = sum / (h2.Count);
                    header.UpdateScoreData(EvaID, sum, tmp2.H_ID,tmp2.Comment);
                    sum2 += sum;
                    sum = 0;
                    
                }
                sum2 = sum2 / (h1.Count);
                header.UpdateScoreData(EvaID, sum2, tmp.H_ID,tmp.Comment);
            }
                if (EvaID>0)
                header.UpdateEvaluationStatus(EvaID, 1);
        }

        [Route("EvaStatus")]
        [HttpPut]
        public void EditEvaluationStatus([FromBody]int EvaID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            var header2 = ServiceContainer.GetService<PesWeb.Service.Modules.EvaManage>();
            tblEvaluation ev = header2.getEvaData().Where(a => a.Eva_ID == EvaID).FirstOrDefault();
            if (ev.EvaStatus == 1)
            {
                header.UpdateEvaluationStatus(EvaID, 2);
            }
            else if (ev.EvaStatus == 2)
            {
                header.UpdateEvaluationStatus(EvaID, 1);
            }

        }

        [Route("GetHeader/{PositionID}")]
        [HttpGet]
        public List<tblHeader> GetHeaderByPosition(int PositionID)
        {
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();
            List<tblHeader> result = header.getHeaderData().ToList();
            List<tblHeader> GetHeader = new List<tblHeader>();
            List<tbllHeaderPosition> Hp = header.getHeaderPosition().Where(a=>a.PositionNO==PositionID).ToList();
            if(Hp.ToList().Count()>0)
            foreach (tblHeader hd in result)
            {
                if(Hp.Where(a=>a.HeaderID == hd.H_ID).ToList().Count == 1)
                {
                        GetHeader.Add(hd);
                }
            }

            List<tblHeader> H = new List<tblHeader>();
            foreach (tblHeader a in GetHeader)
            {
                if (a.Parent == 0)
                {
                    H.Add(a);
                }
                else
                {
                    int parent = (int)a.Parent;
                    for (int i = 0; i < H.Count; i++)
                    {
                        if (H[i].H_ID == parent)
                        {
                            H.Insert(i + 1, a);
                        }
                    }
                }
            }
            List<tblHeader> H_new = new List<tblHeader>();
            foreach (tblHeader a in H)
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

        
        [Route("InsertHeader")]
        [HttpPut]
        public void InsertHeader([FromBody]JObject Data)
        {
            tblHeader H = new tblHeader();
            var header = ServiceContainer.GetService<PesWeb.Service.Modules.HeaderManage>();

            H.Parent = Convert.ToInt32(Data["H_ID"].ToString());
            H.H_Level = (H.Parent==0)?1 : header.GetAllHeader().Where(a => a.H_ID == H.Parent).FirstOrDefault().H_Level + 1;
            H.Text = Data["Text"].ToString();
            H.Text_Eng = Data["Text_Eng"].ToString();
            H.Alias = Data["Alias"].ToString();
            H.PositionNo = Convert.ToInt32( Data["PositionNo"].ToString());

            header.insertHeader(H);
        }

       

        
    }
}
