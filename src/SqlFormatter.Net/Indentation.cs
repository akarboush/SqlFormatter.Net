using SqlFormatter.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFormatter.Net
{
    internal class Indentation
    {
        const short TopLevelId = 1;
        const short BlockLevelId = 2;

        private readonly Indent _indent;
        //TODO: no list
        private readonly List<short> _indentTypes = new();

        public Indentation(Indent indent = Indent.TwoSpaces)
        {
            _indent = indent;
        }

        public string GetIndent()
        {
            if (_indentTypes.Count == 0) return string.Empty;

            return _indent switch
            {
                Indent.TwoSpaces => new string(' ', _indentTypes.Count * 2),
                Indent.FourSpaces => new string(' ', _indentTypes.Count * 4),
                _ => throw new NotImplementedException(),
            };
        }

        public void IncreaseTopLevel() => _indentTypes.Add(TopLevelId);
        public void IncreaseBlockLevel() => _indentTypes.Add(BlockLevelId);

        public void DecreaseTopLevel()
        {
            if (_indentTypes.Count > 0 && _indentTypes.Last() == TopLevelId)
            {
                _indentTypes.RemoveAt(_indentTypes.Count - 1);
            }
        }

        public void DecreaseBlockLevel()
        {
            while (_indentTypes.Count > 0)
            {
                short type = _indentTypes.Last();
                _indentTypes.RemoveAt(_indentTypes.Count - 1);

                if (type != TopLevelId)
                    break;
            }
        }

        public void ResetIndentation()
        {
            _indentTypes.Clear();
        }


    }
}
