using abremir.MSP.Assembler;
using abremir.MSP.Assembler.Models;
using abremir.MSP.Parser;
using abremir.MSP.Parser.Models;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Models;
using abremir.MSP.Validator;
using abremir.MSP.Validator.Models;
using NSubstitute;
using NSubstituteAutoMocker.Standard;

namespace abremir.MSP.Compiler.Test
{
    public class CompilerTests
    {
        private readonly NSubstituteAutoMocker<Compiler> _compiler;

        public CompilerTests()
        {
            _compiler = new NSubstituteAutoMocker<Compiler>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Compile_EmptySource_ReturnsNoSourceDetectedToAssembleError(string? source)
        {
            var parser = _compiler.Get<IParser>();
            var validator = _compiler.Get<IValidator>();
            var assembler = _compiler.Get<IAssembler>();

            var result = _compiler.ClassUnderTest.Compile(source);

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.NoSourceDetectedToAssemble);
            Check.That(result.Warnings).IsEmpty();
            Check.That(result.Data).IsEmpty();
            Check.That(result.Program).IsEmpty();
            parser.DidNotReceive().Parse(Arg.Any<string>());
            validator.DidNotReceive().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.DidNotReceive().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_ParserReturnsErrors_ReturnsEarlyWithErrors()
        {
            var parser = _compiler.Get<IParser>();
            var validator = _compiler.Get<IValidator>();
            var assembler = _compiler.Get<IAssembler>();

            var parserResult = new ParserResult();
            parserResult.AddError(new(Error.SyntaxError));
            parserResult.AddWarning(new(Warning.DataNoVariablesDeclared));

            parser.Parse(Arg.Any<string>()).Returns(parserResult);

            var result = _compiler.ClassUnderTest.Compile("test code");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.SyntaxError);
            Check.That(result.Warnings).IsEmpty();
            Check.That(result.Data).IsEmpty();
            Check.That(result.Program).IsEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.DidNotReceive().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.DidNotReceive().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_ParserOnlyReturnsWarnings()
        {
            var parser = _compiler.Get<IParser>();
            var validator = _compiler.Get<IValidator>();
            var assembler = _compiler.Get<IAssembler>();

            var parserResult = new ParserResult();
            parserResult.AddWarning(new(Warning.DataNoVariablesDeclared));

            parser.Parse(Arg.Any<string>()).Returns(parserResult);
            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(new ValidatorResult());
            assembler.Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(AssemblerResult.Empty());

            var result = _compiler.ClassUnderTest.Compile("test code");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).IsEmpty();
            Check.That(result.Warnings).Not.IsEmpty();
            Check.That(result.Warnings).HasElementThatMatches(warning => warning.Warning == Warning.DataNoVariablesDeclared);
            Check.That(result.Data).IsEmpty();
            Check.That(result.Program).IsEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.Received().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_ValidatorReturnsErrors_ReturnsEarlyWithErrors()
        {
            var parser = _compiler.Get<IParser>();
            var validator = _compiler.Get<IValidator>();
            var assembler = _compiler.Get<IAssembler>();

            parser.Parse(Arg.Any<string>()).Returns(new ParserResult());

            var validatorResult = new ValidatorResult();
            validatorResult.AddError(new(Error.SyntaxError));
            validatorResult.AddWarning(new(Warning.DataNoVariablesDeclared));

            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(validatorResult);

            var result = _compiler.ClassUnderTest.Compile("test code");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.SyntaxError);
            Check.That(result.Warnings).IsEmpty();
            Check.That(result.Data).IsEmpty();
            Check.That(result.Program).IsEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.DidNotReceive().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_ValidatorOnlyReturnsWarnings()
        {
            var parser = _compiler.Get<IParser>();
            var validator = _compiler.Get<IValidator>();
            var assembler = _compiler.Get<IAssembler>();

            var validatorResult = new ValidatorResult();
            validatorResult.AddWarning(new(Warning.DataNoVariablesDeclared));

            parser.Parse(Arg.Any<string>()).Returns(new ParserResult());
            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(validatorResult);
            assembler.Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(AssemblerResult.Empty());

            var result = _compiler.ClassUnderTest.Compile("test code");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).IsEmpty();
            Check.That(result.Warnings).Not.IsEmpty();
            Check.That(result.Warnings).HasElementThatMatches(warning => warning.Warning == Warning.DataNoVariablesDeclared);
            Check.That(result.Data).IsEmpty();
            Check.That(result.Program).IsEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.Received().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_AssemblerReturnsErrors_ReturnsEarlyWithErrors()
        {
            var parser = _compiler.Get<IParser>();
            var validator = _compiler.Get<IValidator>();
            var assembler = _compiler.Get<IAssembler>();

            parser.Parse(Arg.Any<string>()).Returns(new ParserResult());
            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(new ValidatorResult());

            var assemblerResult = new AssemblerResult([], [], [new MspError(Error.SyntaxError)], [], []);

            assembler.Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(assemblerResult);

            var result = _compiler.ClassUnderTest.Compile("test code");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).Not.IsEmpty();
            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.SyntaxError);
            Check.That(result.Warnings).IsEmpty();
            Check.That(result.Data).IsEmpty();
            Check.That(result.Program).IsEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.Received().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_NoErrors()
        {
            var parser = _compiler.Get<IParser>();
            var validator = _compiler.Get<IValidator>();
            var assembler = _compiler.Get<IAssembler>();

            parser.Parse(Arg.Any<string>()).Returns(new ParserResult());
            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(new ValidatorResult());

            var assemblerResult = new AssemblerResult([1, 2], [3, 4], [], [], []);

            assembler.Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(assemblerResult);

            var result = _compiler.ClassUnderTest.Compile("test code");

            Check.That(result).IsNotNull();
            Check.That(result.Errors).IsEmpty();
            Check.That(result.Warnings).IsEmpty();
            Check.That(result.Data).Not.IsEmpty();
            Check.That(result.Data).Is(assemblerResult.Data);
            Check.That(result.Program).Not.IsEmpty();
            Check.That(result.Program).Is(assemblerResult.Program);
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.Received().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }
    }
}