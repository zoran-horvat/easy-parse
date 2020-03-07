namespace EasyParse.Text
{
    public class EndOfText : Location
    {
        private EndOfText() { }

        public static Location Value => new EndOfText();

        public override string ToString() =>
            "EndOfText";

        public override int CompareTo(Location other) =>
            other is EndOfText ? 0 : 1;
    }
}
