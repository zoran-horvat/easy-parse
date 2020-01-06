using System;
using System.Reflection;

namespace EasyParse.CommandLineTool
{
    class Program
    {
        private static string ToolName => Assembly.GetExecutingAssembly().GetName().Name;

        static void Main(string[] args)
        {
            Console.WriteLine(ToolName);
            Console.ReadLine();
        }
    }
}
