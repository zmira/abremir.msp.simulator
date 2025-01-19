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

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.DataSegmentExpected));
        }

        [Fact]
        public void Parse_NoDataSegment_ExpectationIsDataSegmentExpected()
        {
            const string prog = Constants.CodeSegment;
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.DataSegmentExpected));
        }

        [Fact]
        public void Parse_NoCodeSegment_ExpectationIsCodeSegmentExpectedOrInvalidToken()
        {
            const string prog = Constants.DataSegment;
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.CodeSegmentExpectedOrInvalidToken));
        }

        [Fact]
        public void Parse_DataSegmentAndCodeSegment_ReturnsDataAndInstructions()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.ErrorMessage.ShouldBeNull();
            parsedResult.Expectations.ShouldBeNull();
            parsedResult.HasValue.ShouldBeTrue();
            parsedResult.Value.Instructions.ShouldBeEmpty();
            parsedResult.Value.Data.ShouldBeEmpty();
        }

        [Fact]
        public void Parse_MissingDataAddress_ExpectationIsDataUnrecognizedAddress()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.DataUnrecognizedAddress));
        }

        [Fact]
        public void Parse_MissingDataSizeToken_ExpectationIsDataSizeTokenExpected()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg 1{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.DataSizeTokenExpected));
        }

        [Fact]
        public void Parse_MissingDataSize_ExpectationIsDataSizeExpected()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg 1 TAM{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.DataSizeExpected));
        }

        [Fact]
        public void Parse_ExpectationDataUnexpectedInitializationValues()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}gghg 1 TAM 1 VAL{Environment.NewLine}{Constants.CodeSegment}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.DataUnexpectedInitializationValues));
        }

        [Theory]
        [InlineData("")]
        [InlineData("x")]
        public void Parse_InvalidArgumentOnPushValueOperation_ExpectationIsCodePushArgumentOutsideAllowedRange(string argument)
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.PushValue.GetDescription()} {argument}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.CodePushArgumentOutsideAllowedRange));
        }

        [Theory]
        [InlineData(Operation.Add)]
        [InlineData(null)]
        public void Parse_InstructionLineDoesNotEndWithNewLine_ExpectationIsEndOfLineNotFound(Operation? operation)
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.PushValue.GetDescription()} 1 {(operation is null ? string.Empty : operation.GetDescription())}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.EndOfLineNotFound));
        }

        [Fact]
        public void Parse_UnaryOperationWithArgument_ExpectationIsCodeUnexpectedArgument()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}{Operation.Add.GetDescription()} 1";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.CodeUnexpectedArgument));
        }

        [Fact]
        public void Parse_LabelWithoutColon_ExpectationIsCodePotentialLabelWithoutColon()
        {
            var prog = $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine} abc {Operation.PushAddress.GetDescription()}";
            var tokenList = _tokenizer.Tokenize(prog);

            var parsedResult = _tokenListParser.Parse(tokenList);

            parsedResult.HasValue.ShouldBeFalse();
            parsedResult.Expectations.ShouldContain(nameof(Error.CodePotentialLabelWithoutColon));
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

            parsedResult.Data.ShouldNotBeEmpty();
            parsedResult.Instructions.ShouldNotBeEmpty();
            parsedResult.Data!.Count.ShouldBe(4);
            parsedResult.Instructions!.Count.ShouldBe(44);
            parsedResult.Instructions.ShouldContain(code =>
                code.InstructionLabel != null && string.Equals(code.InstructionLabel, "abc", StringComparison.OrdinalIgnoreCase));
            parsedResult.Instructions.ShouldContain(code =>
                code.InstructionLabel != null && string.Equals(code.InstructionLabel, "xyz", StringComparison.OrdinalIgnoreCase));
            parsedResult.Instructions.ShouldContain(code =>
                code.InstructionLabel != null && string.Equals(code.InstructionLabel, "bbb", StringComparison.OrdinalIgnoreCase));
            parsedResult.Instructions.ShouldContain(code =>
                code.InstructionLabel != null && string.Equals(code.InstructionLabel, "zzz", StringComparison.OrdinalIgnoreCase));
        }
    }
}
