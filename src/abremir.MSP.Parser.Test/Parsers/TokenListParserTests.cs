using System;
using abremir.MSP.Parser.Parsers;
using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;

namespace abremir.MSP.Parser.Test.Parsers
{
    public class TokenListParserTests
    {
        private readonly Tokenizer _tokenizer;
        private readonly TokenListParser _tokenListParser;

        public TokenListParserTests()
        {
            _tokenizer = new Tokenizer();
            _tokenListParser = new TokenListParser();
        }

        [Fact]
        public void Parse_NoDataSegmentAndNoCodeSegment_ExpectationIsDataSegmentExpected()
        {
            const string prog = "";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.DataSegmentExpected));
        }

        [Fact]
        public void Parse_NoDataSegment_ExpectationIsDataSegmentExpected()
        {
            const string prog = Constants.CodeSegment;
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.DataSegmentExpected));
        }

        [Fact]
        public void Parse_NoCodeSegment_ExpectationIsCodeSegmentExpectedOrInvalidToken()
        {
            const string prog = Constants.DataSegment;
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.CodeSegmentExpectedOrInvalidToken));
        }

        [Fact]
        public void Parse_DataSegmentAndCodeSegment_ReturnsDataAndInstructions()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.ErrorMessage).IsNull();
            Check.That(parsedResult.Expectations).IsNull();
            Check.That(parsedResult.HasValue).IsTrue();
            Check.That(parsedResult.Value.Instructions).IsEmpty();
            Check.That(parsedResult.Value.Data).IsEmpty();
        }

        [Fact]
        public void Parse_MissingDataAddress_ExpectationIsDataUnrecognizedAddress()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.DataUnrecognizedAddress));
        }

        [Fact]
        public void Parse_MissingDataSizeToken_ExpectationIsDataSizeTokenExpected()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg 1{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.DataSizeTokenExpected));
        }

        [Fact]
        public void Parse_MissingDataSize_ExpectationIsDataSizeExpected()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg 1 TAM{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.DataSizeExpected));
        }

        [Fact]
        public void Parse_ExpectationDataUnexpectedInitializationValues()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg 1 TAM 1 VAL{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.DataUnexpectedInitializationValues));
        }

        [Theory]
        [InlineData("")]
        [InlineData("x")]
        public void Parse_InvalidArgumentOnPushValueOperation_ExpectationIsCodePushArgumentOutsideAllowedRange(string argument)
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.PushValue.GetDescription()} {argument}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.CodePushArgumentOutsideAllowedRange));
        }

        [Theory]
        [InlineData(Operation.Add)]
        [InlineData(null)]
        public void Parse_InstructionLineDoesNotEndWithNewLine_ExpectationIsEndOfLineNotFound(Operation? operation)
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.PushValue.GetDescription()} 1 {(operation is null ? string.Empty : operation.GetDescription())}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.EndOfLineNotFound));
        }

        [Fact]
        public void Parse_UnaryOperationWithArgument_ExpectationIsCodeUnexpectedArgument()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.Add.GetDescription()} 1";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.CodeUnexpectedArgument));
        }

        [Fact]
        public void Parse_LabelWithoutColon_ExpectationIsCodePotentialLabelWithoutColon()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine} abc {Operation.PushAddress.GetDescription()}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            Check.That(parsedResult.HasValue).IsFalse();
            Check.That(parsedResult.Expectations).Contains(nameof(Error.CodePotentialLabelWithoutColon));
        }

        [Fact]
        public void Parse_FullProgram_ReturnsExpectedDataAndInstructions()
        {
            const string prog = @"

;comment

MEMORIA DE DADOS
    x 55 Tam 2 vAl 0 1
    y 66 Tam 1
    z 77 TAMANHO 1 VALORES 5
    W 88 TAM 1 VAL -4

CODIGO;comment

abc:;comment
    PUSH 255
    PUSH -10
    PSHA 123
    PSHA abc;comment
xyz:LOAD ;comment
    LDA
    STORE
    STRA
    IN
    OUT
    INC
    OUTC
    add
    sub
bbb: mul
    div
    ADDa
    and
    or
    not
    EQ
    NE
    LT
    LE
    GT
    GE
    andB
    orB
    notB
zzz:
    jmp abc
    JMP 5000
    JMP -5
    JMP +10
    jmpf abc
    JMPf 5000
    JMPf -5
    JMPf +10
    call abc
    CALL 5000
    CALL -5
    CALL +10
    RET
    NOOP
    HALT
;comment";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList).Value;

            Check.That(parsedResult.Data).Not.IsEmpty();
            Check.That(parsedResult.Instructions).Not.IsEmpty();
            Check.That(parsedResult.Data).CountIs(4);
            Check.That(parsedResult.Instructions).CountIs(44);
            Check.That(parsedResult.Instructions).HasElementThatMatches(code =>
                code.InstructionLabel != null && string.Equals(code.InstructionLabel, "abc", StringComparison.OrdinalIgnoreCase));
            Check.That(parsedResult.Instructions).HasElementThatMatches(code =>
                code.InstructionLabel != null && string.Equals(code.InstructionLabel, "xyz", StringComparison.OrdinalIgnoreCase));
            Check.That(parsedResult.Instructions).HasElementThatMatches(code =>
                code.InstructionLabel != null && string.Equals(code.InstructionLabel, "bbb", StringComparison.OrdinalIgnoreCase));
            Check.That(parsedResult.Instructions).HasElementThatMatches(code =>
                code.InstructionLabel != null && string.Equals(code.InstructionLabel, "zzz", StringComparison.OrdinalIgnoreCase));
        }
    }
}
