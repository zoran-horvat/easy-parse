namespace EasyParse.Parsing.Patterns
{
    class StateIndexAndLabel : StatePattern
    {
        private int StateIndex { get; }
        private string Label { get; }

        public StateIndexAndLabel(int stateIndex, string label)
        {
            StateIndex = stateIndex;
            Label = label;
        }

        public override string ToString() => 
            $"({this.StateIndex}, {this.Label})";

        public override bool Equals(object obj) =>
            obj?.GetType() == this.GetType() &&
            obj is StateIndexAndLabel other &&
            other.StateIndex == this.StateIndex &&
            other.Label.Equals(this.Label);

        public override int GetHashCode() =>
            this.StateIndex ^ this.Label.GetHashCode();
    }
}