using abremir.MSP.Parser.Enums;
using abremir.MSP.Shared.Constants;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;
using System.Text.RegularExpressions;

namespace abremir.MSP.Parser.Parsers
{
    public class Tokenizer : ITokenizer
    {
        public TokenList<MspToken> Tokenize(string source)
        {
            return CreateTokenizer().Tokenize(source);
        }

        private static Superpower.Tokenizer<MspToken> CreateTokenizer()
        {
            var tokenBuilder = new TokenizerBuilder<MspToken>()
                .Match(Span.EqualTo(Environment.NewLine), MspToken.NewLine)
                .Ignore(Comment.ToEndOfLine(Span.EqualTo(";")))
                .Match(Span.EqualToIgnoreCase(Constants.DataSegment), MspToken.DataSegment, true)
                .Match(Span.EqualToIgnoreCase("TAMANHO"), MspToken.Size, true)
                .Match(Span.EqualToIgnoreCase("TAMANH"), MspToken.Size, true)
                .Match(Span.EqualToIgnoreCase("TAMAN"), MspToken.Size, true)
                .Match(Span.EqualToIgnoreCase("TAMA"), MspToken.Size, true)
                .Match(Span.EqualToIgnoreCase("TAM"), MspToken.Size, true)
                .Match(Span.EqualToIgnoreCase("VALORES"), MspToken.Values, true)
                .Match(Span.EqualToIgnoreCase("VALORE"), MspToken.Values, true)
                .Match(Span.EqualToIgnoreCase("VALOR"), MspToken.Values, true)
                .Match(Span.EqualToIgnoreCase("VALO"), MspToken.Values, true)
                .Match(Span.EqualToIgnoreCase("VAL"), MspToken.Values, true)
                .Match(Span.EqualToIgnoreCase(Constants.CodeSegment), MspToken.CodeSegment, true)
                .Match(Span.EqualToIgnoreCase("PUSH"), MspToken.PushValue, true)
                .Match(Span.EqualToIgnoreCase("PSHA"), MspToken.PushAddress, true)
                .Match(Span.EqualToIgnoreCase("LOAD"), MspToken.LoadValue, true)
                .Match(Span.EqualToIgnoreCase("LDA"), MspToken.LoadAddress, true)
                .Match(Span.EqualToIgnoreCase("STORE"), MspToken.StoreValue, true)
                .Match(Span.EqualToIgnoreCase("STRA"), MspToken.StoreAddress, true)
                .Match(Span.EqualToIgnoreCase("INC"), MspToken.InputCharacter, true)
                .Match(Span.EqualToIgnoreCase("IN"), MspToken.InputValue, true)
                .Match(Span.EqualToIgnoreCase("OUTC"), MspToken.OutputCharacter, true)
                .Match(Span.EqualToIgnoreCase("OUT"), MspToken.OutputValue, true)
                .Match(Span.EqualToIgnoreCase("ADDA"), MspToken.AddAddress, true)
                .Match(Span.EqualToIgnoreCase("ADD"), MspToken.Add, true)
                .Match(Span.EqualToIgnoreCase("SUB"), MspToken.Subtract, true)
                .Match(Span.EqualToIgnoreCase("MUL"), MspToken.Multiply, true)
                .Match(Span.EqualToIgnoreCase("DIV"), MspToken.Divide, true)
                .Match(Span.EqualToIgnoreCase("ANDB"), MspToken.BitwiseAnd, true)
                .Match(Span.EqualToIgnoreCase("ORB"), MspToken.BitwiseOr, true)
                .Match(Span.EqualToIgnoreCase("NOTB"), MspToken.BitwiseNot, true)
                .Match(Span.EqualToIgnoreCase("AND"), MspToken.LogicAnd, true)
                .Match(Span.EqualToIgnoreCase("OR"), MspToken.LogicOr, true)
                .Match(Span.EqualToIgnoreCase("NOT"), MspToken.LogicNot, true)
                .Match(Span.EqualToIgnoreCase("EQ"), MspToken.Equal, true)
                .Match(Span.EqualToIgnoreCase("NE"), MspToken.NotEqual, true)
                .Match(Span.EqualToIgnoreCase("LT"), MspToken.LessThan, true)
                .Match(Span.EqualToIgnoreCase("LE"), MspToken.LessThanOrEqual, true)
                .Match(Span.EqualToIgnoreCase("GT"), MspToken.GreaterThan, true)
                .Match(Span.EqualToIgnoreCase("GE"), MspToken.GreaterThanOrEqual, true)
                .Match(Span.EqualToIgnoreCase("JMPF"), MspToken.JumpIfFalse, true)
                .Match(Span.EqualToIgnoreCase("JMP"), MspToken.Jump, true)
                .Match(Span.EqualToIgnoreCase("CALL"), MspToken.Call, true)
                .Match(Span.EqualToIgnoreCase("RET"), MspToken.ReturnFromCall, true)
                .Match(Span.EqualToIgnoreCase("HALT"), MspToken.Halt, true)
                .Match(Span.EqualToIgnoreCase("NOOP"), MspToken.NoOperation, true)
                .Match(Character.EqualTo(':'), MspToken.Colon)
                .Match(Span.Regex(@"(\+|-)\d+"), MspToken.SignedNumber, true)
                .Match(Numerics.IntegerInt32, MspToken.Number, true)
                .Match(Span.Regex(@"\w*[a-z]\w*", RegexOptions.IgnoreCase), MspToken.Text, true)
                .Ignore(Span.WhiteSpace);

            return tokenBuilder.Build();
        }
    }
}
