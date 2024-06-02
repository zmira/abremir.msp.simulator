using System;
using System.Linq;
using abremir.MSP.Parser.Enums;
using abremir.MSP.Parser.Parsers;
using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using Superpower;

namespace abremir.MSP.Parser.Test.Parsers
{
    public class TokenizerTests
    {
        private readonly Tokenizer _tokenizer;

        public TokenizerTests()
        {
            _tokenizer = new Tokenizer();
        }

        [Fact]
        public void Tokenize_NewLine_ReturnsNewLineToken()
        {
            MspToken[] expectedTokens = [MspToken.NewLine];

            var actualTokens = _tokenizer
                .Tokenize(Environment.NewLine)
                .Select(t => t.Kind)
                .ToList();

            expectedTokens.Should().Equal(actualTokens);
        }

        [Theory]
        [InlineData(Constants.DataSegment, new[] { MspToken.DataSegment })]
        [InlineData("abc 0 TAM 1 VAL 2 +3", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.Number, MspToken.SignedNumber })]
        [InlineData("abc 0 TAM 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData("abc 0 TAMA 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData("abc 0 TAMAN 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData("abc 0 TAMANH 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData("abc 0 TAMANHO 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData("abc 0 TAM 4 VALO -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData("abc 0 TAM 4 VALOR -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData("abc 0 TAM 4 VALORE -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData("abc 0 TAM 4 VALORES -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [InlineData(Constants.CodeSegment, new[] { MspToken.CodeSegment })]
        [InlineData("label:", new[] { MspToken.Text, MspToken.Colon })]
        [InlineData(";comment", new MspToken[0])]
        [InlineData("PUSH 7", new[] { MspToken.PushValue, MspToken.Number })]
        [InlineData("PUSH +8", new[] { MspToken.PushValue, MspToken.SignedNumber })]
        [InlineData("PUSH -9", new[] { MspToken.PushValue, MspToken.SignedNumber })]
        [InlineData("PSHA 10", new[] { MspToken.PushAddress, MspToken.Number })]
        [InlineData("PSHA abc", new[] { MspToken.PushAddress, MspToken.Text })]
        [InlineData("LOAD", new[] { MspToken.LoadValue })]
        [InlineData("LDA", new[] { MspToken.LoadAddress })]
        [InlineData("STORE", new[] { MspToken.StoreValue })]
        [InlineData("STRA", new[] { MspToken.StoreAddress })]
        [InlineData("IN", new[] { MspToken.InputValue })]
        [InlineData("OUT", new[] { MspToken.OutputValue })]
        [InlineData("INC", new[] { MspToken.InputCharacter })]
        [InlineData("OUTC", new[] { MspToken.OutputCharacter })]
        [InlineData("ADD", new[] { MspToken.Add })]
        [InlineData("SUB", new[] { MspToken.Subtract })]
        [InlineData("MUL", new[] { MspToken.Multiply })]
        [InlineData("DIV", new[] { MspToken.Divide })]
        [InlineData("ADDA", new[] { MspToken.AddAddress })]
        [InlineData("AND", new[] { MspToken.LogicAnd })]
        [InlineData("OR", new[] { MspToken.LogicOr })]
        [InlineData("NOT", new[] { MspToken.LogicNot })]
        [InlineData("EQ", new[] { MspToken.Equal })]
        [InlineData("NE", new[] { MspToken.NotEqual })]
        [InlineData("LT", new[] { MspToken.LessThan })]
        [InlineData("LE", new[] { MspToken.LessThanOrEqual })]
        [InlineData("GT", new[] { MspToken.GreaterThan })]
        [InlineData("GE", new[] { MspToken.GreaterThanOrEqual })]
        [InlineData("ANDB", new[] { MspToken.BitwiseAnd })]
        [InlineData("ORB", new[] { MspToken.BitwiseOr })]
        [InlineData("NOTB", new[] { MspToken.BitwiseNot })]
        [InlineData("JMP label", new[] { MspToken.Jump, MspToken.Text })]
        [InlineData("JMP 11", new[] { MspToken.Jump, MspToken.Number })]
        [InlineData("JMP +12", new[] { MspToken.Jump, MspToken.SignedNumber })]
        [InlineData("JMP -13", new[] { MspToken.Jump, MspToken.SignedNumber })]
        [InlineData("JMPF label", new[] { MspToken.JumpIfFalse, MspToken.Text })]
        [InlineData("JMPF 14", new[] { MspToken.JumpIfFalse, MspToken.Number })]
        [InlineData("JMPF +15", new[] { MspToken.JumpIfFalse, MspToken.SignedNumber })]
        [InlineData("JMPF -16", new[] { MspToken.JumpIfFalse, MspToken.SignedNumber })]
        [InlineData("CALL label", new[] { MspToken.Call, MspToken.Text })]
        [InlineData("CALL 17", new[] { MspToken.Call, MspToken.Number })]
        [InlineData("CALL +18", new[] { MspToken.Call, MspToken.SignedNumber })]
        [InlineData("CALL -19", new[] { MspToken.Call, MspToken.SignedNumber })]
        [InlineData("RET", new[] { MspToken.ReturnFromCall })]
        [InlineData("HALT", new[] { MspToken.Halt })]
        [InlineData("NOOP", new[] { MspToken.NoOperation })]
        [InlineData("\t   \t", new MspToken[0])]
        public void Tokenize_ReturnsExpectedTokens(string source, MspToken[] expectedTokens)
        {
            var actualTokens = _tokenizer
                .Tokenize(source)
                .Select(t => t.Kind)
                .ToList();

            expectedTokens.Should().Equal(actualTokens);
        }

        [Fact]
        public void Tokenize_InvalidArgumentOnPushValueOperation_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.PushValue.GetDescription()} k(76%";

            Assert.Throws<ParseException>(() => _tokenizer.Tokenize(prog));
        }

        [Fact]
        public void Tokenize_InvalidArgumentOnPushAddressOperation_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.PushAddress.GetDescription()} 0/887&5";

            Assert.Throws<ParseException>(() => _tokenizer.Tokenize(prog));
        }

        [Theory]
        [InlineData(Operation.Jump)]
        [InlineData(Operation.JumpIfFalse)]
        [InlineData(Operation.Call)]
        public void Tokenize_InvalidArgumentOnBranchOperation_ThrowsParserException(Operation operation)
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{operation.GetDescription()} k(76%";

            Assert.Throws<ParseException>(() => _tokenizer.Tokenize(prog));
        }

        [Fact]
        public void Tokenize_GarbageInDataVariable_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}/gghg%{Constants.CodeSegment}";

            Assert.Throws<ParseException>(() => _tokenizer.Tokenize(prog));
        }

        [Fact]
        public void Tokenize_GarbageAfterUnaryOperation_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.ReturnFromCall.GetDescription()} (7lijklsfd";

            Assert.Throws<ParseException>(() => _tokenizer.Tokenize(prog));
        }

        [Fact]
        public void Parse_GarbageInDataValues_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg 1 TAM 1 VAL 1|b%3{Environment.NewLine}{Constants.CodeSegment}";

            Assert.Throws<ParseException>(() => _tokenizer.Tokenize(prog));
        }
    }
}