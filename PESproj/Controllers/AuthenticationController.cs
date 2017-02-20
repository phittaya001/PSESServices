using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PesWeb.Service.Models;
using Newtonsoft.Json.Linq;
using PesWeb.Service.Modules;
using System.Web.Configuration;
using CSI.CastleWindsorHelper;

namespace PESproj.Controllers
{
    public class AuthenticationController : ApiController
    {
        [Route("Login")]
        [HttpPost]
        public EmployeeData LoginDomain([FromBody]JObject LogInData)
        {
            EmployeeData authenRs = null;
            try
            {
                LDAPHelper authen = new LDAPHelper(WebConfigurationManager.AppSettings["ADDomainName"]
                                                        , WebConfigurationManager.AppSettings["ADIPAddress"]
                                                        , Convert.ToInt32(WebConfigurationManager.AppSettings["ADPort"]));

                authenRs = authen.Authenticate(LogInData["username"].ToString(), LogInData["password"].ToString());
            }
            catch (Exception e)
            {
                if (authenRs != null)
                {
                    authenRs.Result = false;
                    authenRs.Message = e.Message;
                    return authenRs;
                }
                else
                {
                    authenRs = new EmployeeData();
                    authenRs.Result = false;
                    authenRs.Message = e.Message;
                    return authenRs;
                }
            }

            if (authenRs.Result == true)
            {
                UserLogSvr svr = ServiceContainer.GetService<UserLogSvr>();
                try
                {
                    svr.EmployeeActivityLog(authenRs.EmployeeID, "LOGIN");
                }
                catch (Exception e)
                {
                    authenRs.Result = false;
                    authenRs.Message = e.Message;
                    return authenRs;
                }
            }
            return authenRs;
        }
    }
}
