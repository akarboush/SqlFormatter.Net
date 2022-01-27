using SqlFormatter.Net.Languages;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class TSqlFormatterTest : BaseFormatterTest
    {

        private static readonly TSqlFormatter _formatter = new TSqlFormatter();

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
            SupportsStrings(_formatter, "\"\"", "''", "N''", "[]");
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
            SupportsOperators(_formatter, "%", "&", "|", "^", "~", "!=", "!<", "!>", "+=", "-=", "*=", "/=", "%=", "|=", "&=", "^=", "::");
        }
        
        [Fact]
        public void SupportsJoinTest()
        {
            SupportsJoin(_formatter, without: new[] { "NATURAL" });
        }


        [Fact]
        public void AdditionalTest()
        {

            // formats INSERT without INTO
            string input = "INSERT Customers (ID, MoneyBalance, Address, City) VALUES (12,-123.4, 'Skagen 2111','Stv');";
            string expectedResult =
@"INSERT
  Customers (ID, MoneyBalance, Address, City)
VALUES
  (12, -123.4, 'Skagen 2111', 'Stv');";
            AssertFormat(_formatter, input, expectedResult);

            // recognizes @variables
            input = "SELECT @variable, @\"var name\", @[var name];";
            expectedResult = "SELECT\r\n  @variable,\r\n  @\"var name\",\r\n  @[var name];";
            AssertFormat(_formatter, input, expectedResult);

            // formats SELECT query with CROSS JOIN
            input = "SELECT a, b FROM t CROSS JOIN t2 on t.id = t2.id_t";
            expectedResult =
@"SELECT
  a,
  b
FROM
  t
  CROSS JOIN t2 on t.id = t2.id_t";
            AssertFormat(_formatter, input, expectedResult);
        }
    }
}
