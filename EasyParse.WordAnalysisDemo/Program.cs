using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.Parsing;

namespace EasyParse.WordAnalysisDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter blocks of text");
            Console.WriteLine("End each block with ^Z");
            Console.WriteLine();

            Parser parser = Parser.FromXmlResource(Assembly.GetExecutingAssembly(), "EasyParse.WordAnalysisDemo.Grammar.xml");
            ISymbolCompiler compiler = new LongestWordsSelector();

            foreach (List<string> text in ReadTexts())
            {
                Console.WriteLine(
                    $"{Environment.NewLine}" +
                    $"Longest words: {parser.Parse(text).Compile(compiler)}" +
                    $"{Environment.NewLine}");
            }
        }

        static IEnumerable<List<string>> ReadTexts()
        {
            while (ReadText().ToList() is List<string> list && list.Any())
                yield return list;
        }

        static IEnumerable<string> ReadText()
        {
            while (Console.ReadLine() is string line)
                yield return line;
        }
    }
}
