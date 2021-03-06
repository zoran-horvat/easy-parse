﻿namespace EasyParse.Text
{
    public class TextLocation : InnerLocation
    {
        public int LineIndex { get; }
        public int LineOffset { get; }
        public TextLocation(int contentOffset, int lineIndex, int lineOffset) : base(contentOffset)
        {
            this.LineIndex = lineIndex;
            this.LineOffset = lineOffset;
        }

        public override string ToString() =>
            $"Ln: {this.LineIndex + 1} Pos: {this.LineOffset + 1}";
    }
}