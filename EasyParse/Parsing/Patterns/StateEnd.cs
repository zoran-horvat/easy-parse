namespace EasyParse.Parsing.Patterns
{
    class StateEnd : StatePattern
    {
        private int StateIndex { get; }
     
        public StateEnd(int stateIndex)
        {
            this.StateIndex = stateIndex;
        }

        public override string ToString() =>
            $"({this.StateIndex}, $)";

        public override bool Equals(object obj) =>
            obj?.GetType() == this.GetType() &&
            obj is StateEnd other &&
            other.StateIndex == this.StateIndex;

        public override int GetHashCode() =>
            this.StateIndex;
    }
}
