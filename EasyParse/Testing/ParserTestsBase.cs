using System.Reflection;

namespace EasyParse.Testing
{
    public abstract class ParserTestsBase
    {
        protected abstract Assembly XmlDefinitionAssembly { get; }
        protected abstract string XmlDefinitionResourceName { get; }
    }
}
