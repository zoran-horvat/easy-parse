using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regex = System.Text.RegularExpressions.Regex;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace EasyParse.Text
{
    public abstract class Plaintext
    {
        public string Content { get; }
        private int Length => this.Content.Length;
        public abstract Location Beginning { get; }

        public string Substring(Location startAt) =>
            startAt is InnerLocation inner && inner.Offset < this.Length ? this.Content.Substring(inner.Offset)
            : string.Empty;

        protected Plaintext(string content)
        {
            this.Content = content;
        }

        public static Plaintext Line(string content) => new Line(content);

        public static Plaintext Text(IEnumerable<string> lines) => new Text(lines);

        public IEnumerable<(RegexMatch match, Location at, Location locationAfter)> TryMatch(Regex pattern, Location at) =>
            at is InnerLocation inner ? this.TryMatch(pattern, inner)
            : Enumerable.Empty<(RegexMatch, Location, Location)>();

        private IEnumerable<(RegexMatch match, Location at, Location locationAfter)> TryMatch(Regex pattern, InnerLocation at) =>
            pattern.Match(this.Content, at.Offset) is RegexMatch match && match.Success ? CreateMatch(pattern, match, at)
            : Enumerable.Empty<(RegexMatch, Location, Location)>();

        private IEnumerable<(RegexMatch match, Location at, Location locationAfter)> CreateMatch(Regex pattern, RegexMatch match, InnerLocation at) => new[]
        {
            (match, this.LocationFor(match.Index), this.LocationFor(match.Index + match.Length))
        };

        protected abstract Location LocationFor(int offset);
    }
}
