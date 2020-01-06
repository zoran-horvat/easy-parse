using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.CommandLineTool.Commands;

namespace EasyParse.CommandLineTool
{
    static class CommandBuilder
    {
        public static Command FromArguments(string[] arguments) =>
            GrammarFile(arguments)
                .Select(grammar => FromGrammar(grammar, arguments))
                .DefaultIfEmpty(new ArgumentsError())
                .First();

        private static Command FromGrammar(FileInfo grammar, string[] arguments) =>
            CompileFile(grammar, arguments)
                .Select(Compile.Create)
                .DefaultIfEmpty(new ArgumentsError())
                .First();
            
        private static IEnumerable<FileInfo> GrammarFile(string[] arguments) =>
            arguments.SelectMany(GrammarFile).Take(1);

        private static string GrammarCommandPattern =>
            @"-grammar=(?<fileName>((""[^\s]+"")|[^\s]+))";

        private static IEnumerable<FileInfo> GrammarFile(string argument) =>
            Regex.Match(argument, GrammarCommandPattern) is Match match && match.Success &&
            match.Groups["fileName"] is Group group && group.Success
                ? new[] {new FileInfo(group.Value) }
                : new FileInfo[0];

        private static IEnumerable<FileInfo> CompileFile(FileInfo grammar, string[] arguments) =>
            arguments.Where(arg => arg == "-compile").Select(_ => grammar).Take(1);
    }
}