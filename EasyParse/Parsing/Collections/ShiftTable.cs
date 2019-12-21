using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using EasyParse.LexicalAnalysis.Tokens;

namespace EasyParse.Parsing.Collections
{
    class ShiftTable
    {
        private IDictionary<(int state, string label), int> StateToNextState { get; }
     
        public ShiftTable(XDocument definition)
        {
            this.StateToNextState = XmlDefinitionUtils.ExtractShift(definition);
        }

        public IEnumerable<int> StateFor(IEnumerator<Token> input, ParsingStack stack) =>
            input.Current is Lexeme lexeme && this.StateToNextState.TryGetValue((stack.StateIndex, lexeme.Label), out int nextState) ? new [] {nextState}
            : Enumerable.Empty<int>();
    }
}
