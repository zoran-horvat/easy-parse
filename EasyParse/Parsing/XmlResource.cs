using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace EasyParse.Parsing
{
    class XmlResource
    {
        private Assembly Assembly { get; }
        private string Name { get; }

        public XmlResource(Assembly assembly, string name)
        {
            this.Assembly = assembly;
            this.Name = name;
        }

        public XDocument Load() => 
            this.Load(this.Assembly);

        private XDocument Load(Assembly assembly) =>
            this.Use(() => assembly.GetManifestResourceStream(this.Name), this.Load);

        private XDocument Load(Stream stream) =>
            stream is null ? new XDocument() : this.SafeLoadDefinition(stream);

        private XDocument SafeLoadDefinition(Stream stream) =>
            this.Use(() => new StreamReader(stream), XDocument.Load);

        private TResult Use<T, TResult>(Func<T> factory, Func<T, TResult> map) where T : IDisposable
        {
            using (T obj = factory())
            {
                return map(obj);
            }
        }
    }
}
