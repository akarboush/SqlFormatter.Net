﻿using SqlFormatter.Net.Models;

namespace SqlFormatter.Net.Languages
{
    // For reference: http://docs.couchbase.com.s3-website-us-west-1.amazonaws.com/server/6.0/n1ql/n1ql-language-reference/index.html
    public sealed class N1qlFormatter : Formatter
    {

        public N1qlFormatter(FormatOptions? formatOptions = default) : base(formatOptions)
        {
        }

        protected override Tokenizer Tokenizer() =>
            new Tokenizer(
                ReservedWords,
                ReservedTopLevelWords,
                ReservedNewlineWords,
                ReservedTopLevelWordsNoIndent,
                stringTypes: new[] { "\"\"", "''", "``" },
                openParens: new[] { "(", "[", "{" },
                closeParens: new[] { ")", "]", "}" },
                indexedPlaceholderTypes: Array.Empty<char>(),
                namedPlaceholderTypes: new[] { '$' },
                lineCommentTypes: new[] { "#", "--" },
                specialWordChars: Array.Empty<string>(),
                operators: new[] { "==", "!=" }
            );

        private string[] ReservedWords => new string[]
            {
                "ALL",
                "ALTER",
                "ANALYZE",
                "AND",
                "ANY",
                "ARRAY",
                "AS",
                "ASC",
                "BEGIN",
                "BETWEEN",
                "BINARY",
                "BOOLEAN",
                "BREAK",
                "BUCKET",
                "BUILD",
                "BY",
                "CALL",
                "CASE",
                "CAST",
                "CLUSTER",
                "COLLATE",
                "COLLECTION",
                "COMMIT",
                "CONNECT",
                "CONTINUE",
                "CORRELATE",
                "COVER",
                "CREATE",
                "DATABASE",
                "DATASET",
                "DATASTORE",
                "DECLARE",
                "DECREMENT",
                "DELETE",
                "DERIVED",
                "DESC",
                "DESCRIBE",
                "DISTINCT",
                "DO",
                "DROP",
                "EACH",
                "ELEMENT",
                "ELSE",
                "END",
                "EVERY",
                "EXCEPT",
                "EXCLUDE",
                "EXECUTE",
                "EXISTS",
                "EXPLAIN",
                "FALSE",
                "FETCH",
                "FIRST",
                "FLATTEN",
                "FOR",
                "FORCE",
                "FROM",
                "FUNCTION",
                "GRANT",
                "GROUP",
                "GSI",
                "HAVING",
                "IF",
                "IGNORE",
                "ILIKE",
                "IN",
                "INCLUDE",
                "INCREMENT",
                "INDEX",
                "INFER",
                "INLINE",
                "INNER",
                "INSERT",
                "INTERSECT",
                "INTO",
                "IS",
                "JOIN",
                "KEY",
                "KEYS",
                "KEYSPACE",
                "KNOWN",
                "LAST",
                "LEFT",
                "LET",
                "LETTING",
                "LIKE",
                "LIMIT",
                "LSM",
                "MAP",
                "MAPPING",
                "MATCHED",
                "MATERIALIZED",
                "MERGE",
                "MISSING",
                "NAMESPACE",
                "NEST",
                "NOT",
                "NULL",
                "NUMBER",
                "OBJECT",
                "OFFSET",
                "ON",
                "OPTION",
                "OR",
                "ORDER",
                "OUTER",
                "OVER",
                "PARSE",
                "PARTITION",
                "PASSWORD",
                "PATH",
                "POOL",
                "PREPARE",
                "PRIMARY",
                "PRIVATE",
                "PRIVILEGE",
                "PROCEDURE",
                "PUBLIC",
                "RAW",
                "REALM",
                "REDUCE",
                "RENAME",
                "RETURN",
                "RETURNING",
                "REVOKE",
                "RIGHT",
                "ROLE",
                "ROLLBACK",
                "SATISFIES",
                "SCHEMA",
                "SELECT",
                "SELF",
                "SEMI",
                "SET",
                "SHOW",
                "SOME",
                "START",
                "STATISTICS",
                "STRING",
                "SYSTEM",
                "THEN",
                "TO",
                "TRANSACTION",
                "TRIGGER",
                "TRUE",
                "TRUNCATE",
                "UNDER",
                "UNION",
                "UNIQUE",
                "UNKNOWN",
                "UNNEST",
                "UNSET",
                "UPDATE",
                "UPSERT",
                "USE",
                "USER",
                "USING",
                "VALIDATE",
                "VALUE",
                "VALUED",
                "VALUES",
                "VIA",
                "VIEW",
                "WHEN",
                "WHERE",
                "WHILE",
                "WITH",
                "WITHIN",
                "WORK",
                "XOR",
            };

        private string[] ReservedTopLevelWords => new string[]
            {
                "DELETE FROM",
                "EXCEPT ALL",
                "EXCEPT",
                "EXPLAIN DELETE FROM",
                "EXPLAIN UPDATE",
                "EXPLAIN UPSERT",
                "FROM",
                "GROUP BY",
                "HAVING",
                "INFER",
                "INSERT INTO",
                "LET",
                "LIMIT",
                "MERGE",
                "NEST",
                "ORDER BY",
                "PREPARE",
                "SELECT",
                "SET CURRENT SCHEMA",
                "SET SCHEMA",
                "SET",
                "UNNEST",
                "UPDATE",
                "UPSERT",
                "USE KEYS",
                "VALUES",
                "WHERE",
            };

        private string[] ReservedTopLevelWordsNoIndent => new string[] { "INTERSECT", "INTERSECT ALL", "MINUS", "UNION", "UNION ALL" };

        private string[] ReservedNewlineWords => new string[]
                    {
                "AND",
                "OR",
                "XOR",
                // joins
                "JOIN",
                "INNER JOIN",
                "LEFT JOIN",
                "LEFT OUTER JOIN",
                "RIGHT JOIN",
                "RIGHT OUTER JOIN",
            };
    }
}