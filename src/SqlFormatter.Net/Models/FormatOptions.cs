using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFormatter.Net.Models
{
    public record FormatOptions
    {
        public string Language { get; init; }
        public Indent Indent { get; init; }
        public bool Uppercase { get; init; }
        public int LinesBetweenQueries { get; init; }
        public Dictionary<string, string>? Params { get; init; }

        public FormatOptions(
            string language = "sql",
            Indent indent = Indent.TwoSpaces,
            bool uppercase = true,
            int linesBetweenQueries = 1,
            Dictionary<string, string>? @params = null)
        {
            Language = language;
            Indent = indent;
            Uppercase = uppercase;
            LinesBetweenQueries = linesBetweenQueries;
            Params = @params;
        }
    }
}
