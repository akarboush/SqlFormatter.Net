using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFormatter.Net.Models
{
    public enum TokenType
    {
        Word,
        String,
        Reserved,
        ReservedTopLevel,
        ReservedTopLevelNoIndent,
        ReservedNewline,
        Operator,
        OpenParen,
        CloseParen,
        LineComment,
        BlockComment,
        Number,
        Placeholder,
    }
}
