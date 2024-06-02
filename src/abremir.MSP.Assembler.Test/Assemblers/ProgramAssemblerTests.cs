using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler.Test.Assemblers
{
    public class ProgramAssemblerTests
    {
        private readonly ProgramAssembler _assembler;

        public ProgramAssemblerTests()
        {
            _assembler = new ProgramAssembler();
        }

        [Fact]
        public void Assemble_EmptyInstructions_ReturnsEmpty()
        {
            var result = _assembler.Assemble([], new Dictionary<string, int>());

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.Program.Should().BeEmpty();
            result.LineAddressMap.Should().BeEmpty();
        }

        [Fact]
        public void Assemble_WithInstructions_ReturnsProgramMemory()
        {
            List<ParsedInstruction> parsedInstructions =
            [
                new(1, Operation.PushValue, 1),
                new(2, Operation.PushAddress, 2),
                new(3, Operation.Halt)
            ];
            var expectedAssembledProgram = new byte[] { 1, 1, 2, 2, 0, 27 };
            List<LineAddress> expectedLineAddressMap = [new(1, 0), new(2, 2), new(3, 5)];

            var result = _assembler.Assemble(parsedInstructions, new Dictionary<string, int>());

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.LineAddressMap.Should().BeEquivalentTo(expectedLineAddressMap);
            result.Program.Should().BeEquivalentTo(expectedAssembledProgram);
        }

        [Fact]
        public void Assemble_WithInstructionsAndLabels_ReturnsProgramMemoryWithMappedLabels()
        {
            List<ParsedInstruction> parsedInstructions =
            [
                new(1, Operation.PushValue, 1, InstructionLabel: "abc"),
                new(2, Operation.Jump, TargetTextIdentifier: "abc"),
                new(3, Operation.JumpIfFalse, TargetTextIdentifier: "abc"),
                new(4, Operation.Call, TargetTextIdentifier: "abc")
            ];
            var expectedAssembledProgram = new byte[] { 1, 1, 23, 0, 0, 24, 0, 0, 25, 0, 0, 27 };
            List<LineAddress> expectedLineAddressMap = [new(1, 0), new(2, 2), new(3, 5), new(4, 8)];

            var result = _assembler.Assemble(parsedInstructions, new Dictionary<string, int>());

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.LineAddressMap.Should().BeEquivalentTo(expectedLineAddressMap);
            result.Program.Should().BeEquivalentTo(expectedAssembledProgram);
        }

        [Fact]
        public void Assemble_WithInstructionsAndRelativeBranchInsideMemoryLimits_ReturnsProgramMemory()
        {
            List<ParsedInstruction> parsedInstructions =
            [
                new(1, Operation.Jump, 5, IsRelative: true),
                new(2, Operation.JumpIfFalse, 10, IsRelative: true),
                new(3, Operation.Call, -1, IsRelative: true)
            ];
            var expectedAssembledProgram = new byte[] { 23, 8, 0, 24, 16, 0, 25, 8, 0, 27 };
            List<LineAddress> expectedLineAddressMap = [new(1, 0), new(2, 3), new(3, 6)];

            var result = _assembler.Assemble(parsedInstructions, new Dictionary<string, int>());

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.LineAddressMap.Should().BeEquivalentTo(expectedLineAddressMap);
            result.Program.Should().BeEquivalentTo(expectedAssembledProgram);
        }

        [Fact]
        public void Assemble_WithInstructionsAndRelativeBranchOutsideMemoryLimits_ReturnsProgramMemoryWithErrors()
        {
            List<ParsedInstruction> parsedInstructions =
            [
                new(1, Operation.Jump, Constants.MemoryCapacity, IsRelative: true),
                new(2, Operation.Jump, 1, IsRelative: true),
                new(3, Operation.Jump, -1, IsRelative: true),
                new(4, Operation.Jump, -50, IsRelative: true),
                new(5, Operation.JumpIfFalse, -50, IsRelative: true),
                new(6, Operation.JumpIfFalse, 1, IsRelative: true),
                new(7, Operation.JumpIfFalse, -1, IsRelative: true),
                new(8, Operation.JumpIfFalse, Constants.MemoryCapacity, IsRelative: true),
                new(9, Operation.Call, -50, IsRelative: true),
                new(10, Operation.Call, 1, IsRelative: true),
                new(11, Operation.Call, -1, IsRelative: true),
                new(12, Operation.Call, Constants.MemoryCapacity, IsRelative: true)
            ];
            var expectedAssembledProgram = new byte[]
            {
                23, 3, 125, 23, 7, 0, 23, 8, 0, 23, 0, 0,
                24, 0, 0, 24, 19, 0, 24, 20, 0, 24, 24, 125,
                25, 0, 0, 25, 31, 0, 25, 32, 0, 25, 36, 125,
                27
            };
            List<LineAddress> expectedLineAddressMap =
            [
                new(1, 0), new(2, 3), new(3, 6), new(4, 9),
                new(5, 12), new(6, 15), new(7, 18), new(8, 21),
                new(9, 24), new(10, 27), new(11, 30), new(12, 33)
            ];
            var expectedLineNumbersWithErrors = new int[] { 1, 4, 5, 8, 9, 12 };

            var result = _assembler.Assemble(parsedInstructions, new Dictionary<string, int>());

            result.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Select(error => error.LineNumber).Should().BeEquivalentTo(expectedLineNumbersWithErrors);
            result.LineAddressMap.Should().BeEquivalentTo(expectedLineAddressMap);
            result.Program.Should().BeEquivalentTo(expectedAssembledProgram);
        }

        [Fact]
        public void Assemble_WithInstructionsAndDataVariables_ReturnsProgramMemoryWithMappedVariables()
        {
            var dataVariableMap = new Dictionary<string, int>
            {
                { "abc", 55 },
                { "xyz", 355 }
            };

            List<ParsedInstruction> parsedInstructions =
            [
                new(1, Operation.PushAddress, TargetTextIdentifier: "abc"),
                new(2, Operation.PushAddress, TargetTextIdentifier: "xyz")
            ];
            var expectedAssembledProgram = new byte[] { 2, 55, 0, 2, 99, 1, 27 };
            List<LineAddress> expectedLineAddressMap = [new(1, 0), new(2, 3)];

            var result = _assembler.Assemble(parsedInstructions, dataVariableMap);

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.LineAddressMap.Should().BeEquivalentTo(expectedLineAddressMap);
            result.Program.Should().BeEquivalentTo(expectedAssembledProgram);
        }

        [Fact]
        public void Assemble_WithoutHaltInstruction_ArtificiallyAddsHaltInstruction()
        {
            List<ParsedInstruction> parsedInstructions =
            [
                new(1, Operation.PushValue, 1)
            ];
            var expectedAssembledProgram = new byte[] { 1, 1, 27 };
            List<LineAddress> expectedLineAddressMap = [new(1, 0)];

            var result = _assembler.Assemble(parsedInstructions, new Dictionary<string, int>());

            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.LineAddressMap.Should().BeEquivalentTo(expectedLineAddressMap);
            result.Program.Should().BeEquivalentTo(expectedAssembledProgram);
        }
    }
}
