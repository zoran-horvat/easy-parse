using System.IO;

namespace EasyParse.CommandLineTool.Commands
{
    abstract class GrammarCommand : Command
    {
        private FileInfo Grammar { get; }
     
        protected GrammarCommand(FileInfo grammar)
        {
            this.Grammar = grammar;
        }

        public override void Execute() =>
            this.Execute(this.Grammar);

        protected abstract void Execute(FileInfo grammar);
    }
}
