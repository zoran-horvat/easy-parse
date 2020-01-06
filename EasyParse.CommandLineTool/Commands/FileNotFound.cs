using System;
using System.IO;

namespace EasyParse.CommandLineTool.Commands
{
    class FileNotFound : Command
    {
        private FileInfo File { get; }

        public FileNotFound(FileInfo file)
        {
            this.File = file;
        }

        public override void Execute() =>
            Console.WriteLine($"Could not find file {this.File.FullName}");
    }
}