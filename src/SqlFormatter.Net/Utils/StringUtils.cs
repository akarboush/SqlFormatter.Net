namespace SqlFormatter.Net.Utils
{
    internal static class StringUtils
    {
        public static string Repeat(this string input, int times)
        {
            return string.Concat(Enumerable.Repeat(input, times));
        }
        
        public static string TrimSpaceEnd(this string input)
        {
            return input.TrimEnd(' ');
        }
    }
}
