using abremir.MSP.Assembler.Models;
using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler.Assemblers
{
    public class ProgramAssembler : IProgramAssembler
    {
        public AssembledProgram Assemble(IReadOnlyCollection<ParsedInstruction> parsedInstructions, IReadOnlyDictionary<string, int> dataVariableMap)
        {
            if (parsedInstructions.Count is 0)
            {
                return AssembledProgram.Empty();
            }

            var programMemory = BuildProgramMemory(parsedInstructions, dataVariableMap);

            return new AssembledProgram(programMemory.Memory, programMemory.Errors, programMemory.LineAddressMap);
        }

        private static (byte[] Memory, ICollection<MspError> Errors, List<LineAddress> LineAddressMap) BuildProgramMemory(IReadOnlyCollection<ParsedInstruction> parsedInstructions, IReadOnlyDictionary<string, int> dataVariableMap)
        {
            var instructionLabelMap = GetInstructionLabelMap(parsedInstructions);
            List<byte> programMemory = [];
            List<MspError> errors = [];
            List<LineAddress> lineNumberMemoryAddressMap = [];
            var dataVariableDictionary = new Dictionary<string, int>(dataVariableMap, StringComparer.OrdinalIgnoreCase);

            foreach (var instruction in parsedInstructions.OrderBy(inst => inst.LineNumber))
            {
                programMemory.Add((byte)instruction.Operation);
                var instructionAddress = programMemory.Count - 1;
                lineNumberMemoryAddressMap.Add(new(instruction.LineNumber, instructionAddress));

                switch (instruction.Operation)
                {
                    case Operation.PushValue:
                        programMemory.Add(instruction.NumericalValue!.Value.ToTwosComplement());
                        break;
                    case Operation.PushAddress:
                        {
                            var address = !string.IsNullOrWhiteSpace(instruction.TargetTextIdentifier)
                                ? dataVariableDictionary[instruction.TargetTextIdentifier]
                                : instruction.NumericalValue!.Value;

                            programMemory.AddRange(address.ToLeastAndMostSignificantBytes());
                        }
                        break;
                    case Operation.Jump
                    or Operation.JumpIfFalse
                    or Operation.Call:
                        {
                            var address = !string.IsNullOrWhiteSpace(instruction.TargetTextIdentifier)
                                ? instructionLabelMap[instruction.TargetTextIdentifier]
                                : instruction.IsRelative ?? false
                                    ? instructionAddress + instruction.Operation.GetNumberOfMemoryCellsOccupied() + instruction.NumericalValue!.Value
                                    : instruction.NumericalValue!.Value;

                            if (address < 0 || address >= Constants.MemoryCapacity)
                            {
                                errors.Add(new(Error.CodeBranchTargetOutsideMemoryLimits, instruction.LineNumber));
                            }

                            programMemory.AddRange(address >= 0 ? address.ToLeastAndMostSignificantBytes() : [0, 0]);
                        }
                        break;
                }
            }

            if (!parsedInstructions.Any(instruction => instruction.Operation is Operation.Halt))
            {
                programMemory.Add((byte)Operation.Halt);
            }

            return (programMemory.ToArray(), errors, lineNumberMemoryAddressMap);
        }

        private static IReadOnlyDictionary<string, int> GetInstructionLabelMap(IReadOnlyCollection<ParsedInstruction> parsedInstructions)
        {
            var instructions = parsedInstructions.OrderBy(instruction => instruction.LineNumber).ToList();

            var programCounter = 0;
            var instructionLabelMap = new Dictionary<string, int>();

            instructions.ForEach(instruction =>
            {
                if (!string.IsNullOrWhiteSpace(instruction.InstructionLabel))
                {
                    instructionLabelMap.Add(instruction.InstructionLabel, programCounter);
                }

                programCounter += instruction.Operation.GetNumberOfMemoryCellsOccupied();
            });

            return instructionLabelMap;
        }
    }
}
