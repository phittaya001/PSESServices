using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Common.Utils
{
    public class CommonUtils
    {
        public static StringComparer GetStringComparer(string culture = "th-TH")
        {
            CultureInfo cultureInfo = new CultureInfo(culture);
            return StringComparer.Create(cultureInfo, false);
        }
    }
}
