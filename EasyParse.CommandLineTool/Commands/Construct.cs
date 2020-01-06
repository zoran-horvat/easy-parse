using System;
using System.IO;
using System.Reflection;
using EasyParse.ParserGenerator;
using EasyParse.ParserGenerator.Models;

namespace EasyParse.CommandLineTool.Commands
{
    class Construct : GrammarCommand
    {
        public Construct(FileInfo grammar) : base(grammar) { }

        protected override void Execute(FileInfo grammar) =>
            Console.WriteLine(this.CreateParser(grammar));

        private ParserDefinition CreateParser(FileInfo grammar) =>
            new GrammarLoader()
                .From(grammar.FullName)
                .BuildParser();
    }
}
