using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.Shared.Models;
using abremir.MSP.Validator.Models;

namespace abremir.MSP.Validator.Validators
{
    internal static class CodeValidator
    {
        public static ValidatorResult Validate(IReadOnlyCollection<ParsedData> parsedData, IReadOnlyCollection<ParsedInstruction> parsedInstructions)
        {
            var validatorResult = new ValidatorResult();

            if (parsedInstructions.Count is 0)
            {
                validatorResult.AddWarning(new(Warning.CodeNoInstructionsDeclared));

                return validatorResult;
            }

            CheckMemoryCapacityExhausted(parsedInstructions, validatorResult);

            CheckRedefinitionOfLabel(parsedInstructions, validatorResult);

            CheckPushArgumentOutsideAllowedRange(parsedInstructions, validatorResult);

            CheckPshaInvalidAddress(parsedInstructions, validatorResult);

            var dataVariables = parsedData.Select(data => data.VariableName).ToList();

            CheckPshaVariableNotDeclared(parsedInstructions, dataVariables, validatorResult);

            CheckPshaInvalidVariableArgument(parsedInstructions, dataVariables, validatorResult);

            CheckBranchUndefinedLabel(parsedInstructions, validatorResult);

            CheckBranchTargetOutsideMemoryLimits(parsedInstructions, validatorResult);

            CheckLabelSelfReference(parsedInstructions, validatorResult);

            CheckUnusedLabel(parsedInstructions, validatorResult);

            CheckLabelSharesNameWithVariable(parsedInstructions, dataVariables, validatorResult);

            CheckAddressDoesNotReferenceVariableSpace(parsedInstructions, parsedData.GetMemoryMap(), validatorResult);

            CheckProgramDoesNotContainHalt(parsedInstructions, validatorResult);

            return validatorResult;
        }

        private static void CheckMemoryCapacityExhausted(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            if (parsedInstructions.Sum(instruction => instruction.Operation.GetNumberOfMemoryCellsOccupied()) >= Constants.MemoryCapacity)
            {
                validatorResult.AddError(new(Error.ProgramMemoryCapacityExhausted));
            }
        }

        private static void CheckRedefinitionOfLabel(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            parsedInstructions
                .Where(instruction => instruction.InstructionLabel is not null)
                .GroupBy(instruction => instruction.InstructionLabel)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.OrderBy(instruction => instruction.LineNumber).Skip(1))
                .ToList()
                .ForEach(instruction => validatorResult.AddError(
                    new(Error.CodeRedefinitionOfLabel, instruction.LineNumber)));
        }

        private static void CheckPushArgumentOutsideAllowedRange(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            parsedInstructions
                .Where(instruction => instruction.Operation is Operation.PushValue
                    && (instruction.NumericalValue < Constants.Min8BitValue || instruction.NumericalValue > Constants.Max8BitValue))
                .ToList()
                .ForEach(instruction => validatorResult.AddError(
                    new(Error.CodePushArgumentOutsideAllowedRange, instruction.LineNumber)));
        }

        private static void CheckPshaInvalidAddress(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            parsedInstructions
                .Where(instruction => instruction.Operation is Operation.PushAddress
                    && instruction.NumericalValue.HasValue
                    && instruction.NumericalValue >= Constants.MemoryCapacity)
                .ToList()
                .ForEach(instruction => validatorResult.AddError(
                    new(Error.CodePshaInvalidAddress, instruction.LineNumber)));
        }

        private static void CheckPshaVariableNotDeclared(IReadOnlyCollection<ParsedInstruction> parsedInstructions, IReadOnlyCollection<string> dataVariables, ValidatorResult validatorResult)
        {
            parsedInstructions
                .Where(instruction => instruction.Operation is Operation.PushAddress
                    && instruction.TargetTextIdentifier is not null
                    && !dataVariables.Contains(instruction.TargetTextIdentifier, StringComparer.OrdinalIgnoreCase))
                .ToList()
                .ForEach(instruction => validatorResult.AddError(
                    new(Error.CodePshaVariableNotDeclared, instruction.LineNumber)));
        }

        private static void CheckPshaInvalidVariableArgument(IReadOnlyCollection<ParsedInstruction> parsedInstructions, IReadOnlyCollection<string> dataVariables, ValidatorResult validatorResult)
        {
            var codeLabels = parsedInstructions
                .Where(instruction => instruction.InstructionLabel is not null)
                .Select(instruction => instruction.InstructionLabel);

            parsedInstructions
                .Where(instruction => instruction.Operation is Operation.PushAddress
                    && instruction.TargetTextIdentifier is not null
                    && !dataVariables.Contains(instruction.TargetTextIdentifier, StringComparer.OrdinalIgnoreCase)
                    && codeLabels.Contains(instruction.TargetTextIdentifier, StringComparer.OrdinalIgnoreCase))
                .ToList()
                .ForEach(instruction => validatorResult.AddError(
                    new(Error.CodePshaInvalidVariableArgument, instruction.LineNumber)));
        }

        private static void CheckBranchUndefinedLabel(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            var codeLabels = parsedInstructions
                .Where(instruction => instruction.InstructionLabel is not null)
                .Select(instruction => instruction.InstructionLabel);

            parsedInstructions
                .Where(instruction => new[] { Operation.Jump, Operation.JumpIfFalse, Operation.Call }.Contains(instruction.Operation)
                    && instruction.TargetTextIdentifier is not null
                    && !codeLabels.Contains(instruction.TargetTextIdentifier, StringComparer.OrdinalIgnoreCase))
                .ToList()
                .ForEach(instruction => validatorResult.AddError(
                    new(Error.CodeBranchUndefinedLabel, instruction.LineNumber)));
        }

        private static void CheckBranchTargetOutsideMemoryLimits(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            parsedInstructions
                .Where(instruction => new[] { Operation.Jump, Operation.JumpIfFalse, Operation.Call }.Contains(instruction.Operation)
                    && instruction.NumericalValue.HasValue
                    && (instruction.NumericalValue < (instruction.IsRelative ?? false ? -1 * (Constants.MemoryCapacity - 1) : 0)
                        || instruction.NumericalValue >= Constants.MemoryCapacity))
                .ToList()
                .ForEach(instruction => validatorResult.AddError(
                    new(Error.CodeBranchTargetOutsideMemoryLimits, instruction.LineNumber)));
        }

        private static void CheckLabelSelfReference(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            parsedInstructions
                .Where(instruction => instruction.InstructionLabel is not null
                    && instruction.TargetTextIdentifier?.Equals(instruction.InstructionLabel, StringComparison.OrdinalIgnoreCase) == true)
                .ToList()
                .ForEach(instruction => validatorResult.AddError(
                    new(Error.CodeBranchInvalidArgument, instruction.LineNumber)));
        }

        private static void CheckUnusedLabel(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            var usedLabels = parsedInstructions
                .Where(instruction => instruction.TargetTextIdentifier is not null)
                .Select(instruction => instruction.TargetTextIdentifier)
                .ToList();

            parsedInstructions
                .Where(instruction => instruction.InstructionLabel is not null
                    && !usedLabels.Contains(instruction.InstructionLabel, StringComparer.OrdinalIgnoreCase))
                .ToList()
                .ForEach(instruction => validatorResult.AddWarning(
                    new(Warning.CodeUnusedLabel, instruction.LineNumber)));
        }

        private static void CheckLabelSharesNameWithVariable(IReadOnlyCollection<ParsedInstruction> parsedInstructions, IReadOnlyCollection<string> dataVariables, ValidatorResult validatorResult)
        {
            parsedInstructions
                .Where(instruction => instruction.InstructionLabel is not null
                    && dataVariables.Contains(instruction.InstructionLabel, StringComparer.OrdinalIgnoreCase))
                .ToList()
                .ForEach(instruction => validatorResult.AddWarning(
                    new(Warning.CodeLabelSharesNameWithVariable, instruction.LineNumber)));
        }

        private static void CheckAddressDoesNotReferenceVariableSpace(IReadOnlyCollection<ParsedInstruction> parsedInstructions, IReadOnlyCollection<byte> dataMemoryMap, ValidatorResult validatorResult)
        {
            parsedInstructions
                .Where(instruction => instruction.Operation is Operation.PushAddress
                    && instruction.NumericalValue.HasValue
                    && (instruction.NumericalValue.Value >= dataMemoryMap.Count || dataMemoryMap.ElementAt(instruction.NumericalValue.Value) is 0))
                .ToList()
                .ForEach(instruction => validatorResult.AddWarning(
                    new(Warning.CodeAddressDoesNotReferenceVariableSpace, instruction.LineNumber)));
        }

        private static void CheckProgramDoesNotContainHalt(IReadOnlyCollection<ParsedInstruction> parsedInstructions, ValidatorResult validatorResult)
        {
            if (!parsedInstructions.Any(instruction => instruction.Operation is Operation.Halt))
            {
                validatorResult.AddWarning(
                    new(Warning.CodeNoHaltInstructionDeclared));
            }
        }
    }
}
