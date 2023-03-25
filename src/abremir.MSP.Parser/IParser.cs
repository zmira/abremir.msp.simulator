using abremir.MSP.Parser.Models;

namespace abremir.MSP.Parser
{
    public interface IParser
    {
        ParserResult Parse(string source);
    }
}
