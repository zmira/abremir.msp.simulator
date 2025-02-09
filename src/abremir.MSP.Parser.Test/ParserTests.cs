using System;
using abremir.MSP.Parser.Parsers;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using NSubstitute;
using NSubstituteAutoMocker.Standard;
using Superpower;
using Superpower.Model;

namespace abremir.MSP.Parser.Test
{
    public class ParserTests
    {
        private readonly NSubstituteAutoMocker<Parser> _parser;

        public ParserTests()
        {
            _parser = new NSubstituteAutoMocker<Parser>();
        }

        [Fact]
        public void Parse_ParseExceptionOfUnknownTypeIsThrownWithoutPosition_ReturnsSyntaxError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var tokenizer = _parser.Get<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, Position.Empty));

            var result = _parser.ClassUnderTest.Parse(string.Empty);

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(
                error => error.Error == Error.SyntaxError
                    && error.ErrorMessage == exceptionMessage);
        }

        [Fact]
        public void Parse_ParseExceptionOfUnknownTypeIsThrown_ReturnsSyntaxError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var errorPosition = new Position(1, 2, 3);
            var tokenizer = _parser.Get<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.ClassUnderTest.Parse(string.Empty);

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(
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
            var tokenizer = _parser.Get<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.ClassUnderTest.Parse($"{operation.GetDescription()} k(76%");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(
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
            var tokenizer = _parser.Get<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.ClassUnderTest.Parse($"{Operation.PushAddress.GetDescription()} 0/887&5");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(
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
            var tokenizer = _parser.Get<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.ClassUnderTest.Parse($"{Operation.PushValue.GetDescription()} k(76%");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(
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
            var tokenizer = _parser.Get<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new ParseException(exceptionMessage, errorPosition));

            var result = _parser.ClassUnderTest.Parse("gghg 1 TAM 1 VAL 1|b%3");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(
                error => error.Error == Error.DataUnexpectedInitializationValues
                    && error.ErrorMessage == exceptionMessage
                    && error.LineNumber == errorPosition.Line
                    && error.ColumnNumber == errorPosition.Column);
        }

        [Fact]
        public void Parse_ExceptionIsThrown_ReturnsExceptionError()
        {
            var exceptionMessage = Guid.NewGuid().ToString();
            var tokenizer = _parser.Get<ITokenizer>();
            tokenizer.Tokenize(Arg.Any<string>()).Returns(_ => throw new Exception(exceptionMessage));

            var result = _parser.ClassUnderTest.Parse(string.Empty);

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(
                error => error.Error == Error.Exception
                    && error.ErrorMessage == exceptionMessage);
        }
    }
}
