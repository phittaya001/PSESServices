using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CSI.Common.Utils
{
    public static class Extensions
    {
        private static string currentCulture = "en-US";
        private static DateTimeFormatInfo enDtFi = new CultureInfo(Extensions.currentCulture, false).DateTimeFormat;
        public static string NullIfEmpty(this string obj)
        {
            return string.IsNullOrEmpty(obj) ? null : obj;
        }

        public static string FormatString(this DateTime dte, string Format)
        {

            return Convert.ToDateTime(dte, enDtFi).ToString(Format);
        }

        public static DateTime ToDateTime(this string str, string Format)
        {
            return DateTime.ParseExact(str, Format, CultureInfo.GetCultureInfo(currentCulture));
        }

        public static Color ToColor(this string HtmlColorCode)
        {
            return ColorTranslator.FromHtml(HtmlColorCode);
        }

        public static string Convert2String(this DateTime dte, string Format)
        {
            string result = Format;
            result = result.Replace("yyyy", dte.ToString("yyyy"));
            result = result.Replace("yy", dte.ToString("yy"));
            result = result.Replace("MMM", dte.ToString("MMM"));
            result = result.Replace("MM", dte.ToString("MM"));
            result = result.Replace("M", dte.ToString("M"));
            result = result.Replace("dd", dte.ToString("dd"));
            result = result.Replace("d", dte.ToString("d"));
            result = result.Replace("HH", dte.ToString("HH"));
            result = result.Replace("hh", dte.ToString("hh"));
            result = result.Replace("H", dte.ToString("%H"));
            result = result.Replace("h", dte.ToString("%h"));
            result = result.Replace("mm", dte.ToString("mm"));
            result = result.Replace("m", dte.ToString("%m"));
            result = result.Replace("ss", dte.ToString("ss"));
            result = result.Replace("s", dte.ToString("%s"));
            return result;
        }

        public static string ReplaceSpecialCharForMessageBox(this string str)
        {
            str = str.Replace("\\r\\n", "\r\n");
            str = str.Replace("\\t", "\t");
            return str;
        }
    }
}
