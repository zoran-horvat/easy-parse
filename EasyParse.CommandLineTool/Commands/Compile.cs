using System;
using System.IO;

namespace EasyParse.CommandLineTool.Commands
{
    class Compile : Command
    {
        private FileInfo Grammar { get; }

        public Compile(FileInfo grammar)
        {
            Grammar = grammar;
        }

        public static Command Create(FileInfo grammar) =>
            grammar.Exists ? (Command)new Compile(grammar) : new FileNotFound(grammar);

        public override void Execute()
        {
            Console.WriteLine($"Compiling {this.Grammar.Name}");
        }
    }
}