using System;
using System.IO;

namespace EasyParse.CommandLineTool.Commands
{
    class Emulate : GrammarCommand
    {
        public Emulate(FileInfo grammar) : base(grammar) { }

        protected override void Execute(FileInfo grammar)
        {
            Console.WriteLine("Emulating...");
        }
    }
}
