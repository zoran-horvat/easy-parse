using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regex = System.Text.RegularExpressions.Regex;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace EasyParse.Text
{
    public class Plaintext
    {
        public string Content { get; }
        public int Length => this.Content.Length;

        public Location Beginning => new LineLocation(0);

        public Plaintext(string content)
        {
            this.Content = content;
        }

        public Plaintext(IEnumerable<string> lines)
            : this(lines.Aggregate(new StringBuilder(), (text, line) => text.Append($"{line}\n")).ToString())
        {
        }

        public IEnumerable<(RegexMatch match, Location at, Location locationAfter)> TryMatch(Regex pattern, Location at) =>
            at is InnerLocation inner ? this.TryMatch(pattern, inner)
            : Enumerable.Empty<(RegexMatch, Location, Location)>();

        private IEnumerable<(RegexMatch match, Location at, Location locationAfter)> TryMatch(Regex pattern, InnerLocation at) =>
            pattern.Match(this.Content, at.Offset) is RegexMatch match && match.Success ? CreateMatch(pattern, match, at)
            : Enumerable.Empty<(RegexMatch, Location, Location)>();

        private IEnumerable<(RegexMatch match, Location at, Location locationAfter)> CreateMatch(Regex pattern, RegexMatch match, InnerLocation at) => new[]
        {
            (match, (Location)new LineLocation(match.Index),
                match.Index + match.Length >= this.Content.Length ? EndOfText.Value
                : new LineLocation(match.Index + match.Length))
        };
    }
}
