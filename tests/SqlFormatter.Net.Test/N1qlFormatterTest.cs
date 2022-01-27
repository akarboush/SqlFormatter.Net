using SqlFormatter.Net.Languages;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class N1qlFormatterTest : BaseFormatterTest
    {
        private static readonly N1qlFormatter _formatter = new N1qlFormatter();


        [Fact]
        public void BehavesLikeSqlTest()
        {
            BehavesLikeSqlFormatter(_formatter);
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
            SupportsOperators(_formatter, "%", "==", "!=");
        }

        [Fact]
        public void SupportsJoinTest()
        {
            SupportsJoin(_formatter, without: new[] { "FULL", "CROSS", "NATURAL" });
        }

        [Fact]
        public void AdditionalTest()
        {
            // formats SELECT query with element selection expression
            string input = "SELECT order_lines[0].productId FROM orders;";
            string expectedResult =
@"SELECT
  order_lines[0].productId
FROM
  orders;";
            AssertFormat(_formatter, input, expectedResult);

            // formats SELECT query with primary key querying
            input = "SELECT fname, email FROM tutorial USE KEYS ['dave', 'ian'];";
            expectedResult =
@"SELECT
  fname,
  email
FROM
  tutorial
USE KEYS
  ['dave', 'ian'];";
            AssertFormat(_formatter, input, expectedResult);

            // formats INSERT with {} object literal
            input = "INSERT INTO heroes (KEY, VALUE) VALUES ('123', {'id':1,'type':'Tarzan'});";
            expectedResult =
@"INSERT INTO
  heroes (KEY, VALUE)
VALUES
  ('123', {'id': 1, 'type': 'Tarzan'});";
            AssertFormat(_formatter, input, expectedResult);

            // formats INSERT with large object and array literals
            input = "INSERT INTO heroes (KEY, VALUE) VALUES ('123', {'id': 1, 'type': 'Tarzan', 'array': [123456789, 123456789, 123456789, 123456789, 123456789], 'hello': 'world'});";
            expectedResult =
@"INSERT INTO
  heroes (KEY, VALUE)
VALUES
  (
    '123',
    {
      'id': 1,
      'type': 'Tarzan',
      'array': [
        123456789,
        123456789,
        123456789,
        123456789,
        123456789
      ],
      'hello': 'world'
    }
  );";
            AssertFormat(_formatter, input, expectedResult);

            // formats SELECT query with UNNEST top level reserver word
            input = "SELECT * FROM tutorial UNNEST tutorial.children c;";
            expectedResult =
@"SELECT
  *
FROM
  tutorial
UNNEST
  tutorial.children c;";
            AssertFormat(_formatter, input, expectedResult);

            // formats SELECT query with NEST and USE KEYS
            input =
@"SELECT * FROM usr
USE KEYS 'Elinor_33313792' NEST orders_with_users orders
ON KEYS ARRAY s.order_id FOR s IN usr.shipped_order_history END;";
            expectedResult =
@"SELECT
  *
FROM
  usr
USE KEYS
  'Elinor_33313792'
NEST
  orders_with_users orders ON KEYS ARRAY s.order_id FOR s IN usr.shipped_order_history END;";
            AssertFormat(_formatter, input, expectedResult);

            // formats explained DELETE query with USE KEYS and RETURNING
            input = "EXPLAIN DELETE FROM tutorial t USE KEYS 'baldwin' RETURNING t";
            expectedResult =
@"EXPLAIN DELETE FROM
  tutorial t
USE KEYS
  'baldwin' RETURNING t";
            AssertFormat(_formatter, input, expectedResult);

            // formats UPDATE query with USE KEYS and RETURNING
            input = "UPDATE tutorial USE KEYS 'baldwin' SET type = 'actor' RETURNING tutorial.type";
            expectedResult =
@"UPDATE
  tutorial
USE KEYS
  'baldwin'
SET
  type = 'actor' RETURNING tutorial.type";
            AssertFormat(_formatter, input, expectedResult);

            // recognizes $variables
            input = "SELECT $variable, $\'var name\', $\"var name\", $`var name`;";
            expectedResult = "SELECT\r\n  $variable,\r\n  $'var name',\r\n  $\"var name\",\r\n  $`var name`;";
            AssertFormat(_formatter, input, expectedResult);

        }
    }
}
