using SqlFormatter.Net.Languages;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class Db2FormatterTest : BaseFormatterTest
    {
        private static readonly Db2Formatter _formatter = new Db2Formatter();

        [Fact]
        public void BehavesLikeSqlTest()
        {
            BehavesLikeSqlFormatter(_formatter);
        }

        [Fact]
        public void SupportsCreateTableTest()
        {
            SupportsCreateTable(_formatter);
        }

        [Fact]
        public void SupportsAlterTableTest()
        {
            SupportsAlterTable(_formatter);
        }

        [Fact]
        public void SupportsStringsTest()
        {
            SupportsStrings(_formatter, "\"\"", "''", "``");
        }

        [Fact]
        public void SupportsBetweenTest()
        {
            SupportsBetween(_formatter);
        }

        [Fact]
        public void SupportsSchemaTest()
        {
            SupportsSchema(_formatter);
        }


        [Fact]
        public void SupportsOperatorsTest()
        {
            SupportsOperators(_formatter, "%", "**", "!=", "!>", "!>", "||");
        }


        [Fact]
        public void AdditionalTest()
        {
            // formats FETCH FIRST like LIMIT
            string input = "SELECT col1 FROM tbl ORDER BY col2 DESC FETCH FIRST 20 ROWS ONLY;";
            string expectedResult =
@"SELECT
  col1
FROM
  tbl
ORDER BY
  col2 DESC
FETCH FIRST
  20 ROWS ONLY;";
            AssertFormat(_formatter, input, expectedResult);

            // formats only -- as a line comment
            input =
@"SELECT col FROM
-- This is a comment
MyTable;";
            expectedResult =
@"SELECT
  col
FROM
  -- This is a comment
  MyTable;";
            AssertFormat(_formatter, input, expectedResult);

            // recognizes @ and # as part of identifiers
            input = "SELECT col#1, @col2 FROM tbl";
            expectedResult =
@"SELECT
  col#1,
  @col2
FROM
  tbl";
            AssertFormat(_formatter, input, expectedResult);

            // recognizes :variables
            input = "SELECT :variable;";
            expectedResult =
@"SELECT
  :variable;";
            AssertFormat(_formatter, input, expectedResult);
        }
    }
}
