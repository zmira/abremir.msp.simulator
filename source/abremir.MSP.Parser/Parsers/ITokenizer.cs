using abremir.MSP.Parser.Enums;
using Superpower.Model;

namespace abremir.MSP.Parser.Parsers
{
    public interface ITokenizer
    {
        public TokenList<MspToken> Tokenize(string source);
    }
}
