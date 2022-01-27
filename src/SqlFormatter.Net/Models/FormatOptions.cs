namespace SqlFormatter.Net.Models
{
    public record struct FormatOptions
    {
        public Indent Indent { get; }
        public bool Uppercase { get; }
        public int LinesBetweenQueries { get; }
        public IReadOnlyDictionary<string, string>? Params { get; }

        public FormatOptions(
            Indent indent = Indent.TwoSpaces,
            bool uppercase = true,
            int linesBetweenQueries = 1,
            IReadOnlyDictionary<string, string>? @params = null)
        {
            Indent = indent;
            Uppercase = uppercase;
            LinesBetweenQueries = linesBetweenQueries;
            Params = @params;
        }
    }
}
