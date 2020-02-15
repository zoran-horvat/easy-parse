namespace EasyParse.Text
{
    class EndOfText : Location
    {
        private EndOfText() { }

        public static Location Value => new EndOfText();

        public override string ToString() =>
            "EndOfText";
    }
}
