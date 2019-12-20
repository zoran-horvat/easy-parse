using System.Collections.Generic;
using System.Xml.Linq;

namespace EasyParse.Parsing.Collections
{
    class ShiftTable
    {
        private IDictionary<(int state, string label), int> StateToNextState { get; }
     
        public ShiftTable(XDocument definition)
        {
            this.StateToNextState = XmlDefinitionUtils.ExtractShift(definition);
        }
    }
}
