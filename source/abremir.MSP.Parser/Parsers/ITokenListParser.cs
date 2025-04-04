using abremir.MSP.Parser.Enums;
using abremir.MSP.Parser.Models;
using Superpower.Model;

namespace abremir.MSP.Parser.Parsers
{
    public interface ITokenListParser
    {
        public TokenListParserResult<MspToken, TokenListParserValue> Parse(TokenList<MspToken> tokens);
    }
}
