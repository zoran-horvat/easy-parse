using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EasyParse.CommandLineTool.Commands
{
    class ArgumentsError : Command
    {
        private static string ToolFullName => Assembly.GetExecutingAssembly().Modules.First().Name;
        private static string ToolFileExtension => new FileInfo(ToolFullName).Extension;
        private static string ToolName => ToolFullName.Substring(0, ToolFullName.Length - ToolFileExtension.Length);

        private static string[] Instructions => new[]
        {
            "",
            "Parser generator/simulator tool",
            "-------------------------------",
            "",
            $"{ToolName} -grammar=<grammar-file>.txt -compile",
            "  Builds an XML file with parser definition based on specified grammar",
            "",
            $"{ToolName} -grammar=<grammar-file>.txt -construct",
            "  Displays parser definition built from the grammar file",
            "",
        };

        private static string InstructionsText => string.Join(Environment.NewLine, Instructions);

        public override void Execute() => Console.WriteLine(InstructionsText);
    }
}