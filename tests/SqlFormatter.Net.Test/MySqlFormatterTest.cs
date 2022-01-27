using SqlFormatter.Net.Languages;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class MySqlFormatterTest : BaseFormatterTest
    {
        private static readonly MySqlFormatter _formatter = new MySqlFormatter();

        [Fact]
        public void BehavesLikeMariaDbTest()
        {
            BehavesLikeMariaDbFormatter(_formatter);
        }

        [Fact]
        public void SupportsOperatorsTest()
        {
            SupportsOperators(_formatter, "->", "->>");
        }
    }
}
