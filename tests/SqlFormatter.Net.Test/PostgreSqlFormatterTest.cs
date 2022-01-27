using SqlFormatter.Net.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class PostgreSqlFormatterTest : BaseFormatterTest
    {
        private static readonly PostgreSqlFormatter _formatter = new PostgreSqlFormatter();

        [Fact]
        public void BehavesLikeSqlTest()
        {
            BehavesLikeSqlFormatter(_formatter);
        }

        [Fact]
        public void SupportsCaseTest()
        {
            SupportsCase(_formatter);
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
            SupportsStrings(_formatter, "\"\"", "''", "U&\"\"", "U&''", "$$");
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
            SupportsOperators(
                _formatter,
                "%",
                "^",
                "!",
                "!!",
                "@",
                "!=",
                "&",
                "|",
                "~",
                "#",
                "<<",
                ">>",
                "||/",
                "|/",
                "::",
                "->>",
                "->",
                "~~*",
                "~~",
                "!~~*",
                "!~~",
                "~*",
                "!~*",
                "!~");
        }


        [Fact]
        public void SupportsJoinTest()
        {
            SupportsJoin(_formatter);
        }

        [Fact]
        public void AdditionalTest()
        {          
           // supports $n placeholders
            string input = "SELECT $1, $2 FROM tbl";
            string expectedResult =
@"SELECT
  $1,
  $2
FROM
  tbl";
            AssertFormat(_formatter, input, expectedResult);

            // supports :name placeholders
            input = "foo = :bar";
            expectedResult = input;
            AssertFormat(_formatter, input, expectedResult);
        }
    }
}
