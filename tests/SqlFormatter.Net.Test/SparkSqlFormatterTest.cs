using SqlFormatter.Net.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class SparkSqlFormatterTest : BaseFormatterTest
    {
        private static readonly SparkSqlFormatter _formatter = new SparkSqlFormatter();

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
            SupportsOperators(_formatter, "!=", "%", "|", "&", "^", "~", "!", "<=>", "%", "&&", "||", "==");
        }

        [Fact]
        public void SupportsJoinTest()
        {
            SupportsJoin(
               _formatter,
               additionally: new[]
               {
                    "ANTI JOIN",
                    "SEMI JOIN",
                    "LEFT ANTI JOIN",
                    "LEFT SEMI JOIN",
                    "RIGHT OUTER JOIN",
                    "RIGHT SEMI JOIN",
                    "NATURAL ANTI JOIN",
                    "NATURAL FULL OUTER JOIN",
                    "NATURAL INNER JOIN",
                    "NATURAL LEFT ANTI JOIN",
                    "NATURAL LEFT OUTER JOIN",
                    "NATURAL LEFT SEMI JOIN",
                    "NATURAL OUTER JOIN",
                    "NATURAL RIGHT OUTER JOIN",
                    "NATURAL RIGHT SEMI JOIN",
                    "NATURAL SEMI JOIN"
               });
        }

        [Fact]
        public void AdditionalTest()
        {           
            // formats WINDOW specification as top level
            string input = "SELECT *, LAG(value) OVER wnd AS next_value FROM tbl WINDOW wnd as (PARTITION BY id ORDER BY time);";
            string expectedResult =
@"SELECT
  *,
  LAG(value) OVER wnd AS next_value
FROM
  tbl
WINDOW
  wnd as (
    PARTITION BY
      id
    ORDER BY
      time
  );";
            AssertFormat(_formatter, input, expectedResult);

            // formats window function and end as inline
            input = "SELECT window(time, \"1 hour\").start AS window_start, window(time, \"1 hour\").end AS window_end FROM tbl;";
            expectedResult = "SELECT\r\n  window(time, \"1 hour\").start AS window_start,\r\n  window(time, \"1 hour\").end AS window_end\r\nFROM\r\n  tbl;";
            AssertFormat(_formatter, input, expectedResult);
        }

    }
}
