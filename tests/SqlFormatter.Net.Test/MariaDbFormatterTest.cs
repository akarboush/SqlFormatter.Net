using SqlFormatter.Net.Languages;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class MariaDbFormatterTest : BaseFormatterTest
    {
        private static readonly MariaDbFormatter _formatter = new MariaDbFormatter();
   
        [Fact]
        public void BehavesLikeMariaDbTest()
        {
            BehavesLikeMariaDbFormatter(_formatter);
        }
    }
}
