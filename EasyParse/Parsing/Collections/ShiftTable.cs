using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing.Collections
{
    class ShiftTable
    {
        private IDictionary<StateIndexAndLabel, int> StateToNextState { get; }
     
        public ShiftTable(XDocument definition)
        {
            this.StateToNextState = XmlDefinitionUtils.ExtractShift(definition);
        }

        public IEnumerable<int> StateFor(IEnumerator<Token> input, ParsingStack stack) =>
            input.Current is Lexeme lexeme && this.StateToNextState.TryGetValue(new StateIndexAndLabel(stack.StateIndex, lexeme.Label), out int nextState) ? new [] {nextState}
            : Enumerable.Empty<int>();
    }
}
