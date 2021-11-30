using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.Fluent;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Native;
using EasyParse.Parsing;
using EasyParse.Text;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.ParserGenerator.Models.Rules;
using System.IO;

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

        private static Compiler<int> BuildReflectionCompiler()
        {
            try
            {
                NativeGrammar grammar = new ReflectionArithmeticGrammar();
                PrintGrammarFile("Reflection-based", grammar.ToGrammarFileContent());
                return grammar.BuildCompiler<int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static Compiler<string> BuildCorrectiveCompiler()
        {
            try
            {
                NativeGrammar grammar = new CorrectiveArithmeticGrammar();
                PrintGrammarFile("Corrective reflection-based", grammar.ToGrammarFileContent());
                return grammar.BuildCompiler<string>();
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
                Compiler<int> fluentCompiler = BuildFluentCompiler();
                Compiler<int> reflectionCompiler = BuildReflectionCompiler();
                Compiler<string> correctiveCompiler = BuildCorrectiveCompiler();

                Console.WriteLine("Enter expressions to evaluate (blank line to exit):");
                foreach (string line in Console.In.ReadLinesUntil(string.Empty))
                {
                    Process("Fluent compiler", fluentCompiler, line);
                    Process("Reflection-based compiler", reflectionCompiler, line);
                    Process("Corrective reflection-based compiler", correctiveCompiler, line);
                }
            }
            catch (TargetInvocationException invocationException)
            {
                Console.WriteLine(invocationException.InnerException);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void Process<T>(string label, Compiler<T> compiler, string line)
        {
            Parser parser = compiler.Parser;
            Console.WriteLine($"Using {label}:");
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
