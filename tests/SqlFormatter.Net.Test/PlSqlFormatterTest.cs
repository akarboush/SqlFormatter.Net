using SqlFormatter.Net.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class PlSqlFormatterTest : BaseFormatterTest
    {
        private static readonly PlSqlFormatter _formatter = new PlSqlFormatter();

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
        public void SupportsAlterTableModifyTest()
        {
            SupportsAlterTableModify(_formatter);
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
            SupportsOperators(_formatter, "||", "**", "!=", ":=");
        }

        [Fact]
        public void SupportsJoinTest()
        {
            SupportsJoin(_formatter);
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
            input = "SELECT col FROM\r\n-- This is a comment\r\nMyTable;\r\n";
            expectedResult =
@"SELECT
  col
FROM
  -- This is a comment
  MyTable;";
            AssertFormat(_formatter, input, expectedResult);

            // recognizes _, $, #, . and @ as part of identifiers
            input = "SELECT my_col$1#, col.2@ FROM tbl\r\n";
            expectedResult =
@"SELECT
  my_col$1#,
  col.2@
FROM
  tbl";
            AssertFormat(_formatter, input, expectedResult);

            // formats INSERT without INTO
            input = "INSERT Customers (ID, MoneyBalance, Address, City) VALUES (12,-123.4, 'Skagen 2111','Stv');";
            expectedResult =
@"INSERT
  Customers (ID, MoneyBalance, Address, City)
VALUES
  (12, -123.4, 'Skagen 2111', 'Stv');";
            AssertFormat(_formatter, input, expectedResult);

            // recognizes ?[0-9]* placeholders
            input = "SELECT ?1, ?25, ?;";
            expectedResult =
@"SELECT
  ?1,
  ?25,
  ?;";
            AssertFormat(_formatter, input, expectedResult);

            // formats SELECT query with CROSS APPLY
            input = "SELECT a, b FROM t CROSS APPLY fn(t.id)";
            expectedResult =
@"SELECT
  a,
  b
FROM
  t
  CROSS APPLY fn(t.id)";
            AssertFormat(_formatter, input, expectedResult);

            // formats simple SELECT
            input = "SELECT N, M FROM t";
            expectedResult =
@"SELECT
  N,
  M
FROM
  t";
            AssertFormat(_formatter, input, expectedResult);

            // formats simple SELECT with national characters
            input = "SELECT N'value'";
            expectedResult =
@"SELECT
  N'value'";
            AssertFormat(_formatter, input, expectedResult);

            // formats SELECT query with OUTER APPLY
            input = "SELECT a, b FROM t OUTER APPLY fn(t.id)";
            expectedResult =
@"SELECT
  a,
  b
FROM
  t
  OUTER APPLY fn(t.id)";
            AssertFormat(_formatter, input, expectedResult);

            // formats Oracle recursive sub queries
            input =
@"WITH t1(id, parent_id) AS (
  -- Anchor member.
  SELECT
    id,
    parent_id
  FROM
    tab1
  WHERE
    parent_id IS NULL
  MINUS
    -- Recursive member.
  SELECT
    t2.id,
    t2.parent_id
  FROM
    tab1 t2,
    t1
  WHERE
    t2.parent_id = t1.id
) SEARCH BREADTH FIRST BY id SET order1,
another AS (SELECT * FROM dual)
SELECT id, parent_id FROM t1 ORDER BY order1;";
            expectedResult =
@"WITH t1(id, parent_id) AS (
  -- Anchor member.
  SELECT
    id,
    parent_id
  FROM
    tab1
  WHERE
    parent_id IS NULL
  MINUS
  -- Recursive member.
  SELECT
    t2.id,
    t2.parent_id
  FROM
    tab1 t2,
    t1
  WHERE
    t2.parent_id = t1.id
) SEARCH BREADTH FIRST BY id SET order1,
another AS (
  SELECT
    *
  FROM
    dual
)
SELECT
  id,
  parent_id
FROM
  t1
ORDER BY
  order1;";
            AssertFormat(_formatter, input, expectedResult);

            // formats Oracle recursive sub queries regardless of capitalization
            input =
@"WITH t1(id, parent_id) AS (
  -- Anchor member.
  SELECT
    id,
    parent_id
  FROM
    tab1
  WHERE
    parent_id IS NULL
  MINUS
    -- Recursive member.
  SELECT
    t2.id,
    t2.parent_id
  FROM
    tab1 t2,
    t1
  WHERE
    t2.parent_id = t1.id
) SEARCH BREADTH FIRST by id set order1,
another AS (SELECT * FROM dual)
SELECT id, parent_id FROM t1 ORDER BY order1;";
            expectedResult =
@"WITH t1(id, parent_id) AS (
  -- Anchor member.
  SELECT
    id,
    parent_id
  FROM
    tab1
  WHERE
    parent_id IS NULL
  MINUS
  -- Recursive member.
  SELECT
    t2.id,
    t2.parent_id
  FROM
    tab1 t2,
    t1
  WHERE
    t2.parent_id = t1.id
) SEARCH BREADTH FIRST by id set order1,
another AS (
  SELECT
    *
  FROM
    dual
)
SELECT
  id,
  parent_id
FROM
  t1
ORDER BY
  order1;";
            AssertFormat(_formatter, input, expectedResult);
        }

    }
}
