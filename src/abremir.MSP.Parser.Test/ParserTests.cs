using System;
using abremir.MSP.Parser.Parsers;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using NSubstitute;
using Superpower;
using Superpower.Model;
using Tethos.NSubstitute;

namespace abremir.MSP.Parser.Test
{
    public class ParserTests : AutoMockingTest
    {
        private readonly Parser _parser;

        public ParserTests()
        {
            _parser = Container.Resolve<Parser>();
        }

        [Fact]
        public void Parse_ParseExceptionOfUnknownTypeIsThrownWithoutPosition_ReturnsSyntaxError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var tokenizer = Container.Resolve<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, Position.Empty));

            var result = _parser.Parse(string.Empty);

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(
                error => error.Error == Error.SyntaxError
                    && error.ErrorMessage == exceptionMessage);
        }

        [Fact]
        public void Parse_ParseExceptionOfUnknownTypeIsThrown_ReturnsSyntaxError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var errorPosition = new Position(1, 2, 3);
            var tokenizer = Container.Resolve<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.Parse(string.Empty);

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(
                error => error.Error == Error.SyntaxError
                    && error.ErrorMessage == exceptionMessage
                    && error.LineNumber == errorPosition.Line
                    && error.ColumnNumber == errorPosition.Column);
        }

        [Theory]
        [InlineData(Operation.Jump)]
        [InlineData(Operation.JumpIfFalse)]
        [InlineData(Operation.Call)]
        public void Parse_ParseExceptionForBranchOperation_ReturnsBranchInvalidArgumentError(Operation operation)
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var errorPosition = new Position(1, 1, 1);
            var tokenizer = Container.Resolve<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.Parse($"{operation.GetDescription()} k(76%");

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(
                error => error.Error == Error.CodeBranchInvalidArgument
                    && error.ErrorMessage == exceptionMessage
                    && error.LineNumber == errorPosition.Line
                    && error.ColumnNumber == errorPosition.Column);
        }

        [Fact]
        public void Parse_ParseExceptionForPushAddressOperation_ReturnsPshaInvalidArgumentError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var errorPosition = new Position(1, 1, 1);
            var tokenizer = Container.Resolve<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.Parse($"{Operation.PushAddress.GetDescription()} 0/887&5");

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(
                error => error.Error == Error.CodePshaInvalidArgument
                    && error.ErrorMessage == exceptionMessage
                    && error.LineNumber == errorPosition.Line
                    && error.ColumnNumber == errorPosition.Column);
        }

        [Fact]
        public void Parse_ParseExceptionForPushValueOperation_ReturnsPushArgumentOutsideAllowedRangeError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var errorPosition = new Position(1, 1, 1);
            var tokenizer = Container.Resolve<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.Parse($"{Operation.PushValue.GetDescription()} k(76%");

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(
                error => error.Error == Error.CodePushArgumentOutsideAllowedRange
                    && error.ErrorMessage == exceptionMessage
                    && error.LineNumber == errorPosition.Line
                    && error.ColumnNumber == errorPosition.Column);
        }

        [Fact]
        public void Parse_ParseExceptionForDataDeclaration_ReturnsUnexpectedInitializationValuesError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var errorPosition = new Position(1, 1, 1);
            var tokenizer = Container.Resolve<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.Parse("gghg 1 TAM 1 VAL 1|b%3");

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(
                error => error.Error == Error.DataUnexpectedInitializationValues
                    && error.ErrorMessage == exceptionMessage
                    && error.LineNumber == errorPosition.Line
                    && error.ColumnNumber == errorPosition.Column);
        }

        [Fact]
        public void Parse_ExceptionIsThrown_ReturnsExceptionError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var tokenizer = Container.Resolve<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new Exception(exceptionMessage));

            var result = _parser.Parse(string.Empty);

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(
                error => error.Error == Error.Exception
                    && error.ErrorMessage == exceptionMessage);
        }
    }
}
