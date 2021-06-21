using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyParse.Text;

namespace EasyParse.Parsing.Nodes.Errors
{
    public class CompileError : Error
    {
        public CompileError(Location location, string label, IEnumerable<object> arguments)
            : base(location, FormatMapping(label, arguments))
        {
        }

        public CompileError(Location location, string label, params object[] arguments)
            : this(location, label, (IEnumerable<object>)arguments)
        {
        }

        private static string FormatMapping(string label, IEnumerable<object> arguments)
        {
            using (IEnumerator<object> enumerator = arguments.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return label;

                StringBuilder list = new StringBuilder(enumerator.Current.ToString());

                while (enumerator.MoveNext())
                {
                    list.Append(" ").Append(enumerator.Current);
                }

                return $"{list} -> {label}";
            }
        }
    }
}