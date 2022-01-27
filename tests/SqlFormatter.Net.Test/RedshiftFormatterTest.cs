using SqlFormatter.Net.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class RedshiftFormatterTest : BaseFormatterTest
    {
        private static readonly RedshiftFormatter _formatter = new RedshiftFormatter();

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
        public void SupportsSchemaTest()
        {
            SupportsSchema(_formatter);
        }

        [Fact]
        public void SupportsOperatorsTest()
        {
            SupportsOperators(_formatter, "%", "^", "|/", "||/", "<<", ">>", "&", "|", "~", "!", "!=", "||");
        }

        [Fact]
        public void SupportsJoinTest()
        {
            SupportsJoin(_formatter);
        }

        [Fact]
        public void ÁdditionalTest()
        {
            // formats LIMIT
            string input = "SELECT col1 FROM tbl ORDER BY col2 DESC LIMIT 10;";
            string expectedResult =
@"SELECT
  col1
FROM
  tbl
ORDER BY
  col2 DESC
LIMIT
  10;";
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

            // recognizes @ as part of identifiers
            input = @"SELECT @col1 FROM tbl";
            expectedResult =
@"SELECT
  @col1
FROM
  tbl";
            AssertFormat(_formatter, input, expectedResult);

            // formats DISTKEY and SORTKEY after CREATE TABLE
            input = @"CREATE TABLE items (a INT PRIMARY KEY, b TEXT, c INT NOT NULL, d INT NOT NULL) DISTKEY(created_at) SORTKEY(created_at);";
            expectedResult =
@"CREATE TABLE items (a INT PRIMARY KEY, b TEXT, c INT NOT NULL, d INT NOT NULL)
DISTKEY
(created_at)
SORTKEY
(created_at);";
            AssertFormat(_formatter, input, expectedResult);

            // formats COPY
            input =
@"COPY schema.table
FROM 's3://bucket/file.csv'
IAM_ROLE 'arn:aws:iam::123456789:role/rolename'
FORMAT AS CSV DELIMITER ',' QUOTE ''
REGION AS 'us-east-1'";
            expectedResult =
@"COPY
  schema.table
FROM
  's3://bucket/file.csv'
IAM_ROLE
  'arn:aws:iam::123456789:role/rolename'
FORMAT
  AS CSV
DELIMITER
  ',' QUOTE ''
REGION
  AS 'us-east-1'";
            AssertFormat(_formatter, input, expectedResult);
        }

    }
}
