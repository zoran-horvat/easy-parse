using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParserCompiler
{
    public class GrammarLoader
    {
        private IServiceProvider ServiceProvider { get; }

        public GrammarLoader(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IEnumerable<string> From(string fileName) =>
            this.RawLines(fileName)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("#"));

        private IEnumerable<string> RawLines(string fileName) =>
            this.GrammarItem(fileName).FileNames[0] is string filePath && File.Exists(filePath) ? File.ReadAllLines(filePath)
            : new string[0];

        private EnvDTE.ProjectItem GrammarItem(string fileName) =>
            this.VisualStudio.Solution.FindProjectItem(fileName);

        private EnvDTE.DTE VisualStudio =>
            this.ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
    }
}
