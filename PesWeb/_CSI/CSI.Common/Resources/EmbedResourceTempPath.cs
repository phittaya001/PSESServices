using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace CSI.Common.Resources
{
    public sealed class EmbedResourceTempPath : IDisposable
    {
        public string TempPath { get; private set; }

        public EmbedResourceTempPath(string resource)
        {
            Load(resource);
        }

        private void Load(string resource)
        {
            string[] names = resource.Split(new char[] { ',' });
            string ResName;
            string AssemName;
            Stream stream = null;

            if (names.Length == 2)
            {
                ResName = names[0].Trim();
                AssemName = names[1].Trim();

                Assembly assembly = null;
                assembly = Assembly.Load(names[1].Trim());
                stream = assembly.GetManifestResourceStream(AssemName + "." + ResName);
                if (stream == null)
                    stream = assembly.GetManifestResourceStream(ResName);
            }
            else
            {
                ResName = resource.Trim();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    stream = assembly.GetManifestResourceStream(ResName);
                    if (stream != null)
                        break;
                }
            }

            if (stream != null)
            {
                TempPath = Path.GetTempFileName();
                using (StreamWriter sw = new StreamWriter(TempPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(stream);
                    doc.Save(sw);
                }
            }
        }

        public void Dispose()
        {
            File.Delete(TempPath);
        }
    }



}
