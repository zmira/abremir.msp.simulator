using abremir.MSP.Assembler;
using abremir.MSP.Assembler.Models;
using abremir.MSP.Parser;
using abremir.MSP.Parser.Models;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Models;
using abremir.MSP.Validator;
using abremir.MSP.Validator.Models;
using NSubstitute;
using Tethos.NSubstitute;

namespace abremir.MSP.Compiler.Test
{
    public class CompilerTests : AutoMockingTest
    {
        private readonly Compiler _compiler;

        public CompilerTests()
        {
            _compiler = Container.Resolve<Compiler>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Compile_EmptySource_ReturnsNoSourceDetectedToAssembleError(string? source)
        {
            var parser = Container.Resolve<IParser>();
            var validator = Container.Resolve<IValidator>();
            var assembler = Container.Resolve<IAssembler>();

            var result = _compiler.Compile(source);

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(error => error.Error == Error.NoSourceDetectedToAssemble);
            result.Warnings.Should().BeEmpty();
            result.Data.Should().BeEmpty();
            result.Program.Should().BeEmpty();
            parser.DidNotReceive().Parse(Arg.Any<string>());
            validator.DidNotReceive().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.DidNotReceive().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_ParserReturnsErrors_ReturnsEarlyWithErrors()
        {
            var parser = Container.Resolve<IParser>();
            var validator = Container.Resolve<IValidator>();
            var assembler = Container.Resolve<IAssembler>();

            var parserResult = new ParserResult();
            parserResult.AddError(new(Error.SyntaxError));
            parserResult.AddWarning(new(Warning.DataNoVariablesDeclared));

            parser.Parse(Arg.Any<string>()).Returns(parserResult);

            var result = _compiler.Compile("test code");

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(error => error.Error == Error.SyntaxError);
            result.Warnings.Should().BeEmpty();
            result.Data.Should().BeEmpty();
            result.Program.Should().BeEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.DidNotReceive().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.DidNotReceive().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_ParserOnlyReturnsWarnings()
        {
            var parser = Container.Resolve<IParser>();
            var validator = Container.Resolve<IValidator>();
            var assembler = Container.Resolve<IAssembler>();

            var parserResult = new ParserResult();
            parserResult.AddWarning(new(Warning.DataNoVariablesDeclared));

            parser.Parse(Arg.Any<string>()).Returns(parserResult);
            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(new ValidatorResult());
            assembler.Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(AssemblerResult.Empty());

            var result = _compiler.Compile("test code");

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.Warnings.Should().NotBeEmpty();
            result.Warnings.Should().Contain(warning => warning.Warning == Warning.DataNoVariablesDeclared);
            result.Data.Should().BeEmpty();
            result.Program.Should().BeEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.Received().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_ValidatorReturnsErrors_ReturnsEarlyWithErrors()
        {
            var parser = Container.Resolve<IParser>();
            var validator = Container.Resolve<IValidator>();
            var assembler = Container.Resolve<IAssembler>();

            parser.Parse(Arg.Any<string>()).Returns(new ParserResult());

            var validatorResult = new ValidatorResult();
            validatorResult.AddError(new(Error.SyntaxError));
            validatorResult.AddWarning(new(Warning.DataNoVariablesDeclared));

            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(validatorResult);

            var result = _compiler.Compile("test code");

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(error => error.Error == Error.SyntaxError);
            result.Warnings.Should().BeEmpty();
            result.Data.Should().BeEmpty();
            result.Program.Should().BeEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.DidNotReceive().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_ValidatorOnlyReturnsWarnings()
        {
            var parser = Container.Resolve<IParser>();
            var validator = Container.Resolve<IValidator>();
            var assembler = Container.Resolve<IAssembler>();

            var validatorResult = new ValidatorResult();
            validatorResult.AddWarning(new(Warning.DataNoVariablesDeclared));

            parser.Parse(Arg.Any<string>()).Returns(new ParserResult());
            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(validatorResult);
            assembler.Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(AssemblerResult.Empty());

            var result = _compiler.Compile("test code");

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.Warnings.Should().NotBeEmpty();
            result.Warnings.Should().Contain(warning => warning.Warning == Warning.DataNoVariablesDeclared);
            result.Data.Should().BeEmpty();
            result.Program.Should().BeEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.Received().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_AssemblerReturnsErrors_ReturnsEarlyWithErrors()
        {
            var parser = Container.Resolve<IParser>();
            var validator = Container.Resolve<IValidator>();
            var assembler = Container.Resolve<IAssembler>();

            parser.Parse(Arg.Any<string>()).Returns(new ParserResult());
            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(new ValidatorResult());

            var assemblerResult = new AssemblerResult([], [], [new MspError(Error.SyntaxError)], [], []);

            assembler.Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(assemblerResult);

            var result = _compiler.Compile("test code");

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(error => error.Error == Error.SyntaxError);
            result.Warnings.Should().BeEmpty();
            result.Data.Should().BeEmpty();
            result.Program.Should().BeEmpty();
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.Received().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }

        [Fact]
        public void Compile_NoErrors()
        {
            var parser = Container.Resolve<IParser>();
            var validator = Container.Resolve<IValidator>();
            var assembler = Container.Resolve<IAssembler>();

            parser.Parse(Arg.Any<string>()).Returns(new ParserResult());
            validator.Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(new ValidatorResult());

            var assemblerResult = new AssemblerResult([1, 2], [3, 4], [], [], []);

            assembler.Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>()).Returns(assemblerResult);

            var result = _compiler.Compile("test code");

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.Warnings.Should().BeEmpty();
            result.Data.Should().NotBeEmpty();
            result.Data.Should().BeEquivalentTo(assemblerResult.Data);
            result.Program.Should().NotBeEmpty();
            result.Program.Should().BeEquivalentTo(assemblerResult.Program);
            parser.Received().Parse(Arg.Any<string>());
            validator.Received().Validate(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
            assembler.Received().Assemble(Arg.Any<IReadOnlyCollection<ParsedData>>(), Arg.Any<IReadOnlyCollection<ParsedInstruction>>());
        }
    }
}