using CSI.ModelHelper.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Common.Repositories
{
    public class DbMessageBoxRepository
    {
        [EnableCache(CacheBehavior.Singleton, CacheTags.MessageBox)]
        public virtual Dictionary<string, string> LoadMessages()
        {
            throw new NotImplementedException();
        }
    }
}
