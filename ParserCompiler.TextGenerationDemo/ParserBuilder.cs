using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using EasyParse.LexicalAnalysis;
using EasyParse.Parsing;

namespace ParserCompiler.TextGenerationDemo
{
    class ParserBuilder
    {
        private string ResourceName { get; }

        public ParserBuilder(string resourceName)
        {
            this.ResourceName = resourceName;
        }

        public Parser Build() => 
            Parser.From(this.LoadDefinition(), this.CreateLexer());

        private Lexer CreateLexer() =>
            new Lexer()
                .AddPattern(@"\d+", "n")
                .AddPattern(@"\+", "+")
                .IgnorePattern(@"\s+");

        private XDocument LoadDefinition() => 
            this.LoadDefinition(Assembly.GetExecutingAssembly());

        private XDocument LoadDefinition(Assembly assembly) =>
            this.Use(() => assembly.GetManifestResourceStream(this.ResourceName), this.LoadDefinition);

        private XDocument LoadDefinition(Stream stream) =>
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