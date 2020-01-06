using System;

namespace EasyParse.CommandLineTool
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                CommandBuilder.FromArguments(args).Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
