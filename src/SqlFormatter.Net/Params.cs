using SqlFormatter.Net.Models;

namespace SqlFormatter.Net
{
    public class Params
    {
        private readonly Dictionary<string, string>? _params;
        private int _index = 0;

        public Params(Dictionary<string, string>? @params)
        {
            _params = @params;
        }

        public string Get(TokenWithKey token)
        {
            if (_params is null)
                return token.Value;

            var key = token.KeyParser(token.Value);

            if (!string.IsNullOrEmpty(key))
            {
                return _params.TryGetValue(key, out var paramValue) ? paramValue : token.Value;
            }

            return _params.ElementAtOrDefault(_index).Value ?? token.Value;
        }
    }
}
