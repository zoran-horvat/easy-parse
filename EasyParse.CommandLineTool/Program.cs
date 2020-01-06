namespace EasyParse.CommandLineTool
{
    class Program
    {

        static void Main(string[] args)
        {
            CommandBuilder.FromArguments(args).Execute();
        }
    }
}
