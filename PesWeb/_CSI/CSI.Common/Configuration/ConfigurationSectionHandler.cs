using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace CSI.Common.Configuration
{
    public class CsiConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            string typeName = null == section.Attributes["type"] ? "" : section.Attributes["type"].InnerText;
            Type type = Type.GetType(typeName);
            if (null == type)
                return null;

            XmlNodeList paramList = section.SelectNodes("param");
            Hashtable param = new Hashtable();
            foreach (XmlNode n in paramList)
            {
                if (null != n.Attributes["name"] && null != n.Attributes["value"])
                    param[n.Attributes["name"].InnerText] = n.Attributes["value"].InnerText;
            }

            object obj = null;
            object[] objParam = null;
            ConstructorInfo constructor = null;
            if (param.Count > 0)
            {
                Type[] types = { typeof(IDictionary) };
                objParam = new object[1] { param };
                constructor = type.GetConstructor(types);
            }

            if (null == constructor)
            {
                Type[] types = { };
                objParam = new object[0];
                constructor = type.GetConstructor(types);
            }

            if (null != constructor)
                obj = constructor.Invoke(objParam);
            else
            {
                throw new MissingMethodException(string.Format("Cannot find an appropriated constructor of type '{0}'.", typeName));
            }
            return obj;
        }
    }

    public class CsiConfigurationPropLoader
    {
        public static void LoadProp(Type theClass, string configPath)
        {
            ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();
            configFile.ExeConfigFilename = configPath;

            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
            AppSettingsSection section = config.GetSection("constant.classes/" + theClass.FullName) as AppSettingsSection;

            PropertyInfo[] props = theClass.GetProperties();
            foreach (PropertyInfo p in props)
            {
                KeyValueConfigurationElement e = section.Settings[p.Name];
                if (null != e)
                {
                    string v = e.Value;
                    string typeName = p.PropertyType.FullName;

                    if (string.IsNullOrEmpty(v))
                        p.SetValue(null, null, null);
                    else if (typeName.Contains(typeof(string).Name))
                    {
                        p.SetValue(null, v, null);
                    }
                    else if (typeName.Contains(typeof(Int16).Name))
                    {
                        p.SetValue(null, Int16.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Int32).Name))
                    {
                        p.SetValue(null, Int32.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Int64).Name))
                    {
                        p.SetValue(null, Int64.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Single).Name))
                    {
                        p.SetValue(null, Single.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Double).Name))
                    {
                        p.SetValue(null, Double.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Decimal).Name))
                    {
                        p.SetValue(null, Decimal.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(DateTime).Name))
                    {
                        try
                        {
                            p.SetValue(null, DateTime.ParseExact(v, "d/m/yyyy", null), null);
                            continue;
                        }
                        catch { }

                        try
                        {
                            p.SetValue(null, DateTime.ParseExact(v, "yyyy/m/d", null), null);
                            continue;
                        }
                        catch { }

                        p.SetValue(null, DateTime.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Byte).Name))
                    {
                        p.SetValue(null, Byte.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Boolean).Name))
                    {
                        p.SetValue(null, Boolean.Parse(v), null);
                    }
                }
            }
        }
    }
}