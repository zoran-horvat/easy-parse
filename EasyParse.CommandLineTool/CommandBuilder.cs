using System;
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
                .DefaultIfEmpty(ArgumentsError())
                .First()
                .Invoke();

        private static Func<Command> FromGrammar(FileInfo grammar, string[] arguments) =>
            grammar.Exists ? FromExistingGrammar(grammar, arguments)
            : GrammarNotFound(grammar);

        private static Func<Command> FromExistingGrammar(FileInfo grammar, string[] arguments) =>
            Compile(grammar, arguments)
                .Concat(Construct(grammar, arguments))
                .DefaultIfEmpty(ArgumentsError())
                .First();

        private static Func<Command> GrammarNotFound(FileInfo grammar) => () => new FileNotFound(grammar);
        private static Func<Command> ArgumentsError() => () => new ArgumentsError();
        private static Func<Command> Compile(FileInfo grammar) => () => new Compile(grammar);
        private static Func<Command> Construct(FileInfo grammar) => () => new Construct(grammar);

        private static IEnumerable<FileInfo> GrammarFile(string[] arguments) =>
            arguments.SelectMany(GrammarFile).Take(1);

        private static string GrammarCommandPattern =>
            @"-grammar=(?<fileName>((""[^\s]+"")|[^\s]+))";

        private static IEnumerable<FileInfo> GrammarFile(string argument) =>
            Regex.Match(argument, GrammarCommandPattern) is Match match && match.Success &&
            match.Groups["fileName"] is Group group && group.Success
                ? new[] {new FileInfo(group.Value) }
                : new FileInfo[0];

        private static IEnumerable<Func<Command>> Compile(FileInfo grammar, string[] arguments) =>
            CommandFlagFor(grammar, arguments, "-compile").Select(Compile);

        private static IEnumerable<Func<Command>> Construct(FileInfo grammar, string[] arguments) =>
            CommandFlagFor(grammar, arguments, "-construct").Select(Construct);

        private static IEnumerable<FileInfo> CommandFlagFor(FileInfo grammar, string[] arguments, string flag) =>
            arguments.Where(arg => arg == flag).Select(_ => grammar);
    }
}