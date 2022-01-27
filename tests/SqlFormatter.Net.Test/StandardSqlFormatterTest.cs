using SqlFormatter.Net;
using SqlFormatter.Net.Languages;
using SqlFormatter.Net.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace SqlFormatter.Net.Test
{
    public class StandardSqlFormatterTest : BaseFormatterTest
    {
        private static readonly StandardSqlFormatter _formatter = new StandardSqlFormatter();

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
        public void SupportsCaseTest()
        {
            SupportsCase(_formatter);
        }
        
        [Fact]
        public void SupportsStringsTest()
        {
            SupportsStrings(_formatter, "\"\"", "''");
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
        public void SupportsJoinTest()
        {
            SupportsJoin(_formatter);
        }

        [Fact]
        public void AdditionalTest()
        {
            // formats FETCH FIRST like LIMIT
            string input = "SELECT * FETCH FIRST 2 ROWS ONLY;";
            string expectedResult =
@"SELECT
  *
FETCH FIRST
  2 ROWS ONLY;";
            AssertFormat(_formatter, input, expectedResult);
        }     
    }
}
