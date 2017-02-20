using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesWeb.Service.Common
{
    public interface IMessageBoxSvc
    {
        string ResolveMessage(string msgId, params object[] args);
    }
}
