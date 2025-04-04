using System;
using System.Linq;
using abremir.MSP.Parser.Enums;
using abremir.MSP.Parser.Parsers;
using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using NSubstitute.ExceptionExtensions;
using Superpower;

namespace abremir.MSP.Parser.Test.Parsers
{
    [TestClass]
    public class TokenizerTests
    {
        private readonly Tokenizer _tokenizer;

        public TokenizerTests()
        {
            _tokenizer = new Tokenizer();
        }

        [TestMethod]
        public void Tokenize_NewLine_ReturnsNewLineToken()
        {
            MspToken[] expectedTokens = [MspToken.NewLine];

            var actualTokens = _tokenizer
                .Tokenize(Environment.NewLine)
                .Select(t => t.Kind)
                .ToList();

            Check.That(expectedTokens).IsEqualTo(actualTokens);
        }

        [TestMethod]
        [DataRow(Constants.DataSegment, new[] { MspToken.DataSegment })]
        [DataRow("abc 0 TAM 1 VAL 2 +3", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.Number, MspToken.SignedNumber })]
        [DataRow("abc 0 TAM 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow("abc 0 TAMA 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow("abc 0 TAMAN 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow("abc 0 TAMANH 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow("abc 0 TAMANHO 4 VAL -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow("abc 0 TAM 4 VALO -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow("abc 0 TAM 4 VALOR -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow("abc 0 TAM 4 VALORE -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow("abc 0 TAM 4 VALORES -5 6", new[] { MspToken.Text, MspToken.Number, MspToken.Size, MspToken.Number, MspToken.Values, MspToken.SignedNumber, MspToken.Number })]
        [DataRow(Constants.CodeSegment, new[] { MspToken.CodeSegment })]
        [DataRow("label:", new[] { MspToken.Text, MspToken.Colon })]
        [DataRow(";comment", new MspToken[0])]
        [DataRow("PUSH 7", new[] { MspToken.PushValue, MspToken.Number })]
        [DataRow("PUSH +8", new[] { MspToken.PushValue, MspToken.SignedNumber })]
        [DataRow("PUSH -9", new[] { MspToken.PushValue, MspToken.SignedNumber })]
        [DataRow("PSHA 10", new[] { MspToken.PushAddress, MspToken.Number })]
        [DataRow("PSHA abc", new[] { MspToken.PushAddress, MspToken.Text })]
        [DataRow("LOAD", new[] { MspToken.LoadValue })]
        [DataRow("LDA", new[] { MspToken.LoadAddress })]
        [DataRow("STORE", new[] { MspToken.StoreValue })]
        [DataRow("STRA", new[] { MspToken.StoreAddress })]
        [DataRow("IN", new[] { MspToken.InputValue })]
        [DataRow("OUT", new[] { MspToken.OutputValue })]
        [DataRow("INC", new[] { MspToken.InputCharacter })]
        [DataRow("OUTC", new[] { MspToken.OutputCharacter })]
        [DataRow("ADD", new[] { MspToken.Add })]
        [DataRow("SUB", new[] { MspToken.Subtract })]
        [DataRow("MUL", new[] { MspToken.Multiply })]
        [DataRow("DIV", new[] { MspToken.Divide })]
        [DataRow("ADDA", new[] { MspToken.AddAddress })]
        [DataRow("AND", new[] { MspToken.LogicAnd })]
        [DataRow("OR", new[] { MspToken.LogicOr })]
        [DataRow("NOT", new[] { MspToken.LogicNot })]
        [DataRow("EQ", new[] { MspToken.Equal })]
        [DataRow("NE", new[] { MspToken.NotEqual })]
        [DataRow("LT", new[] { MspToken.LessThan })]
        [DataRow("LE", new[] { MspToken.LessThanOrEqual })]
        [DataRow("GT", new[] { MspToken.GreaterThan })]
        [DataRow("GE", new[] { MspToken.GreaterThanOrEqual })]
        [DataRow("ANDB", new[] { MspToken.BitwiseAnd })]
        [DataRow("ORB", new[] { MspToken.BitwiseOr })]
        [DataRow("NOTB", new[] { MspToken.BitwiseNot })]
        [DataRow("JMP label", new[] { MspToken.Jump, MspToken.Text })]
        [DataRow("JMP 11", new[] { MspToken.Jump, MspToken.Number })]
        [DataRow("JMP +12", new[] { MspToken.Jump, MspToken.SignedNumber })]
        [DataRow("JMP -13", new[] { MspToken.Jump, MspToken.SignedNumber })]
        [DataRow("JMPF label", new[] { MspToken.JumpIfFalse, MspToken.Text })]
        [DataRow("JMPF 14", new[] { MspToken.JumpIfFalse, MspToken.Number })]
        [DataRow("JMPF +15", new[] { MspToken.JumpIfFalse, MspToken.SignedNumber })]
        [DataRow("JMPF -16", new[] { MspToken.JumpIfFalse, MspToken.SignedNumber })]
        [DataRow("CALL label", new[] { MspToken.Call, MspToken.Text })]
        [DataRow("CALL 17", new[] { MspToken.Call, MspToken.Number })]
        [DataRow("CALL +18", new[] { MspToken.Call, MspToken.SignedNumber })]
        [DataRow("CALL -19", new[] { MspToken.Call, MspToken.SignedNumber })]
        [DataRow("RET", new[] { MspToken.ReturnFromCall })]
        [DataRow("HALT", new[] { MspToken.Halt })]
        [DataRow("NOOP", new[] { MspToken.NoOperation })]
        [DataRow("\t   \t", new MspToken[0])]
        public void Tokenize_ReturnsExpectedTokens(string source, MspToken[] expectedTokens)
        {
            var actualTokens = _tokenizer
                .Tokenize(source)
                .Select(t => t.Kind)
                .ToList();

            Check.That(expectedTokens).IsEqualTo(actualTokens);
        }

        [TestMethod]
        public void Tokenize_InvalidArgumentOnPushValueOperation_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.PushValue.GetDescription()} k(76%";

            Check.ThatCode(() => _tokenizer.Tokenize(prog)).Throws<ParseException>();
        }

        [TestMethod]
        public void Tokenize_InvalidArgumentOnPushAddressOperation_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.PushAddress.GetDescription()} 0/887&5";

            Check.ThatCode(() => _tokenizer.Tokenize(prog)).Throws<ParseException>();
        }

        [TestMethod]
        [DataRow(Operation.Jump)]
        [DataRow(Operation.JumpIfFalse)]
        [DataRow(Operation.Call)]
        public void Tokenize_InvalidArgumentOnBranchOperation_ThrowsParserException(Operation operation)
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{operation.GetDescription()} k(76%";

            Check.ThatCode(() => _tokenizer.Tokenize(prog)).Throws<ParseException>();
        }

        [TestMethod]
        public void Tokenize_GarbageInDataVariable_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}/gghg%{Constants.CodeSegment}";

            Check.ThatCode(() => _tokenizer.Tokenize(prog)).Throws<ParseException>();
        }

        [TestMethod]
        public void Tokenize_GarbageAfterUnaryOperation_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.ReturnFromCall.GetDescription()} (7lijklsfd";

            Check.ThatCode(() => _tokenizer.Tokenize(prog)).Throws<ParseException>();
        }

        [TestMethod]
        public void Parse_GarbageInDataValues_ThrowsParserException()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg 1 TAM 1 VAL 1|b%3{Environment.NewLine}{Constants.CodeSegment}";

            Check.ThatCode(() => _tokenizer.Tokenize(prog)).Throws<ParseException>();
        }
    }
}