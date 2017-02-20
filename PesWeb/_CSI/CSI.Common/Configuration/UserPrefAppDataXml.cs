using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSI.Common.Configuration
{
    public class UserPrefAppDataXml : UserPreferenceBase
    {
        public UserPrefAppDataXml(string appName)
            : base(appName)
        {
        }
        public override void Load()
        {
            lock (DataTable)
            {
                DataTable.Clear();
                string path = GetPath();
                if (false == File.Exists(path))
                    return;

                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode root = doc.SelectSingleNode("/preference");
                if (null == root)
                    throw new XmlException("Root node not found.");

                string app = root.Attributes["app-name"] == null ? string.Empty : root.Attributes["app-name"].Value;
                if (string.Compare(app, AppName, true) != 0)
                    throw new XmlException("Mismatch application preference file.");

                XmlNodeList nodes = root.SelectNodes("add");
                foreach (XmlNode node in nodes)
                {
                    string key = node.Attributes["key"].Value;
                    string value = node.Attributes["value"].Value;
                    DataTable[key] = value;
                }
            }
        }
        public override void Save()
        {
            lock (DataTable)
            {
                XmlDocument doc = new XmlDocument();
                var root = doc.CreateNode("element", "preference", "");
                doc.AppendChild(root);
                var a = doc.CreateAttribute("app-name");
                a.Value = AppName;
                root.Attributes.Append(a);

                foreach (var key in DataTable.Keys)
                {
                    var value = DataTable[key];
                    if (null == value)
                        continue;

                    var node = doc.CreateNode("element", "add", "");
                    var a1 = doc.CreateAttribute("key");
                    a1.Value = key;
                    var a2 = doc.CreateAttribute("value");
                    if (value.GetType() == typeof(DateTime))
                        a2.Value = ((DateTime)value).ToString(CultureInfo.InvariantCulture);
                    else
                        a2.Value = DataTable[key].ToString();
                    node.Attributes.Append(a1);
                    node.Attributes.Append(a2);
                    root.AppendChild(node);
                }
                string path = GetPath();
                using (XmlTextWriter writer = new XmlTextWriter(path, null))
                {
                    writer.Formatting = Formatting.Indented;
                    doc.Save(writer);
                }
            }
        }
        private string GetPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appDataPath, AppName);
            if (false == Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return Path.Combine(folder, "userpreference.xml");
        }
    }
}
