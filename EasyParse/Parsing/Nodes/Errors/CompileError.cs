﻿using System.Collections.Generic;
using System.Linq;
using EasyParse.Text;

namespace EasyParse.Parsing.Nodes.Errors
{
    public class CompileError : Error
    {
        public CompileError(string label, IEnumerable<object> arguments)
            : base(new LineLocation(0), FormatMapping(label, arguments))
        {
        }

        private static string FormatMapping(string label, IEnumerable<object> arguments) =>
            string.Join(" ", arguments.Select(x => $"{x}").ToArray()) + $" -> {label}";
    }
}