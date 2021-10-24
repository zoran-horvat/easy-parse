using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.Fluent;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Native;
using EasyParse.Parsing;
using EasyParse.Text;

namespace EasyParse.CalculatorDemo
{
    public static class Program
    {
        private static void PrintGrammarFile(string label, IEnumerable<string> content)
        {
            Console.WriteLine($"{label} grammar:");
            Console.WriteLine();
            Console.WriteLine(string.Join(Environment.NewLine, content));
            Console.WriteLine(new string('-', 80));
        }

        private static Compiler<int> BuildFluentCompiler()
        {
            try
            {
                FluentGrammar grammar = new ArithmeticGrammar();
                PrintGrammarFile("Fluent", grammar.ToGrammarFileContent());
                return grammar.BuildCompiler<int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static void BuildReflectionCompiler()
        {
            try
            {
                ReflectionGrammar grammar = new ReflectionArithmeticGrammar();
                PrintGrammarFile("Reflection-based", grammar.ToGrammarFileContent());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                Compiler<int> compiler = BuildFluentCompiler();
                Parser parser = compiler.Parser;

                BuildReflectionCompiler();

                Parser addingParser = Parser.FromXmlResource(Assembly.GetExecutingAssembly(), "EasyParse.CalculatorDemo.AdditionGrammar.xml");

                Console.WriteLine("Enter expressions to evaluate (blank line to exit):");
                foreach (string line in Console.In.ReadLinesUntil(string.Empty))
                {
                    Process(parser, compiler, line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Process<T>(Parser parser, Compiler<T> compiler, string line)
        {
            List<Token> tokens = parser.Lexer.Tokenize(Plaintext.Line(line)).ToList();
            string tokensReport = string.Join(" ", tokens.Select(x => $"{x}"));
            
            Console.WriteLine();
            Console.WriteLine($"Tokens: {tokensReport}");
            
            ParsingResult result = parser.Parse(line);
            Console.WriteLine();
            Console.WriteLine($"Syntax tree:{Environment.NewLine}{result}");

            Console.WriteLine();
            CompilationResult<T> compiled = compiler.Compile(line);
            Console.WriteLine($"{line} = {compiled}");
            Console.WriteLine(new string('-', 50));
        }

        private static ISymbolCompiler Calculator => new Calculator();
    }
}
