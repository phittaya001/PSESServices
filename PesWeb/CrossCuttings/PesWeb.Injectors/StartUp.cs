using CSI.ModelHelper.Cache;
using CSI.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Injectors
{
    public class StartUp
    {
        public static void Initial()
        {
            CacheContext.Start();
            SecurityModelCrypto.Sault = "PesWeb";
            SecurityModelCrypto.ByPass = false;
            ServiceRegister.Register();
        }
    }
}
