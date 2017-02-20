using CSI.CastleWindsorHelper;
using PesWeb.Service.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Common
{
    public class DbMessageBoxSvc : IMessageBoxSvc
    {
        public string ResolveMessage(string msgId, params object[] args)
        {
            DbMessageBoxRepository rep = ServiceContainer.GetService<DbMessageBoxRepository>();
            var map = rep.LoadMessages();
            string resolved;
            if (false == map.TryGetValue(msgId, out resolved))
                resolved = msgId;
            return args == null ? resolved : string.Format(resolved, args);
        }
    }
}
