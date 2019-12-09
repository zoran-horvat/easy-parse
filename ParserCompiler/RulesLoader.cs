using System;
using System.IO;

namespace ParserCompiler
{
    public class RulesLoader
    {
        private IServiceProvider ServiceProvider { get; }

        public RulesLoader(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public string[] LoadFrom(string fileName) =>
            this.GrammarItem(fileName).FileNames[0] is string filePath && File.Exists(filePath) ? File.ReadAllLines(filePath)
            : new string[0];

        private EnvDTE.ProjectItem GrammarItem(string fileName) =>
            this.VisualStudio.Solution.FindProjectItem(fileName);

        private EnvDTE.DTE VisualStudio =>
            this.ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
    }
}
