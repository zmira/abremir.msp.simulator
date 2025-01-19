using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Validator.Test.Validators
{
    public class CodeValidatorTests
    {
        [Fact]
        public void Validate_NoCodeDeclarations_ReturnsMemoryEmptyWarning()
        {
            var result = CodeValidator.Validate([], []);

            result.Errors.ShouldBeEmpty();
            result.Warnings.Single().Warning.ShouldBe(Warning.CodeNoInstructionsDeclared);
        }

        [Fact]
        public void Validate_CodeInstructionsExceedMemoryCapacity_ReturnsMemoryCapacityExhaustedError()
        {
            List<ParsedInstruction> code = [];

            for (var i = 0; i < (Constants.MemoryCapacity / Operation.PushAddress.GetNumberOfMemoryCellsOccupied()) + 1; i++)
            {
                code.Add(new(i + 1, Operation.PushAddress, i));
            }

            var result = CodeValidator.Validate([], code);

            result.Errors.ShouldContain(error => error.Error == Error.ProgramMemoryCapacityExhausted);
        }

        [Fact]
        public void Validate_LabelExistsAndIsRedefined_ReturnsRedefinitionOfLabelError()
        {
            var code1 = new ParsedInstruction(1, Operation.Halt);
            var code2 = new ParsedInstruction(2, Operation.Halt);
            var codeX1 = new ParsedInstruction(3, Operation.Halt, InstructionLabel: "x");
            var codeX2 = new ParsedInstruction(4, Operation.Halt, InstructionLabel: "x");
            var codeX3 = new ParsedInstruction(5, Operation.Halt, InstructionLabel: "x");

            var result = CodeValidator.Validate([], [codeX3, code2, codeX1, code1, codeX2]);

            result.Errors.Where(error => error.Error is Error.CodeRedefinitionOfLabel)
                .Select(error => error.LineNumber).ToArray().ShouldBeEquivalentTo(new[] { codeX2.LineNumber, codeX3.LineNumber });
        }

        [Fact]
        public void Validate_PushOperationValuesAreOutsideAllowedRange_ReturnsPushArgumentOutsideAllowedRangeError()
        {
            var code1 = new ParsedInstruction(1, Operation.PushValue, Constants.Min8BitValue - 1);
            var code2 = new ParsedInstruction(2, Operation.PushValue, Constants.Min8BitValue);
            var code3 = new ParsedInstruction(3, Operation.PushValue, Constants.Max8BitValue + 1);
            var code4 = new ParsedInstruction(4, Operation.PushValue, Constants.Max8BitValue);
            var code5 = new ParsedInstruction(5, Operation.PushValue, 3);
            var code6 = new ParsedInstruction(5, Operation.PushValue, -3);

            var result = CodeValidator.Validate([], [code5, code2, code4, code1, code3, code6]);

            result.Errors.Where(error => error.Error is Error.CodePushArgumentOutsideAllowedRange)
                .Select(error => error.LineNumber).ToArray().ShouldBeEquivalentTo(new[] { code1.LineNumber, code3.LineNumber });
        }

        [Fact]
        public void Validate_PushAddressOperationValueIsBeyondMemoryCapacity_ReturnsCodePshaInvalidAddressError()
        {
            var code = new ParsedInstruction(0, Operation.PushAddress, Constants.MemoryCapacity);

            var result = CodeValidator.Validate([], [code]);

            result.Errors.ShouldContain(error => error.Error == Error.CodePshaInvalidAddress);
        }

        [Fact]
        public void Validate_PushAddressOperationReferencesUndeclaredVariable_ReturnsCodePshaVariableNotDeclaredError()
        {
            var code = new ParsedInstruction(0, Operation.PushAddress, TargetTextIdentifier: "x");

            var result = CodeValidator.Validate([], [code]);

            result.Errors.ShouldContain(error => error.Error == Error.CodePshaVariableNotDeclared);
        }

        [Fact]
        public void Validate_PushAddressOperationReferencesLabelInsteadOfVariable_ReturnsCodePshaInvalidVariableArgumentError()
        {
            var code1 = new ParsedInstruction(1, Operation.PushValue, 1, InstructionLabel: "y");
            var code2 = new ParsedInstruction(2, Operation.PushAddress, TargetTextIdentifier: "y");

            var result = CodeValidator.Validate([], [code1, code2]);

            result.Errors.Single(error => error.Error is Error.CodePshaInvalidVariableArgument).LineNumber.ShouldBe(code2.LineNumber);
        }

        [Theory]
        [InlineData(Operation.Jump)]
        [InlineData(Operation.JumpIfFalse)]
        [InlineData(Operation.Call)]
        public void Validate_BranchLabelNotDefined_ReturnsCodeBranchUndefinedLabelError(Operation operation)
        {
            var code = new ParsedInstruction(1, operation, TargetTextIdentifier: "y");

            var result = CodeValidator.Validate([], [code]);

            result.Errors.ShouldContain(error => error.Error == Error.CodeBranchUndefinedLabel);
        }

        [Theory]
        [InlineData(Operation.Jump)]
        [InlineData(Operation.JumpIfFalse)]
        [InlineData(Operation.Call)]
        public void Validate_BranchAddressBeyondMemoryCapacity_ReturnsCodeBranchTargetOutsideMemoryLimitsError(Operation operation)
        {
            var code1 = new ParsedInstruction(1, operation, Constants.MemoryCapacity);
            var code2 = new ParsedInstruction(2, operation, -1);
            var code3 = new ParsedInstruction(3, operation, 0);

            var result = CodeValidator.Validate([], [code2, code3, code1]);

            result.Errors.Where(error => error.Error is Error.CodeBranchTargetOutsideMemoryLimits)
                .Select(error => error.LineNumber).ToArray().ShouldBeSubsetOf(new[] { code1.LineNumber, code2.LineNumber });
        }

        [Theory]
        [InlineData(Operation.Jump)]
        [InlineData(Operation.JumpIfFalse)]
        [InlineData(Operation.Call)]
        public void Validate_RelativeBranchAddressBeyondMemoryCapacity_ReturnsCodeBranchTargetOutsideMemoryLimitsError(Operation operation)
        {
            var code1 = new ParsedInstruction(1, operation, Constants.MemoryCapacity, IsRelative: true);
            var code2 = new ParsedInstruction(2, operation, -1 * Constants.MemoryCapacity, IsRelative: true);
            var code3 = new ParsedInstruction(3, operation, +1, IsRelative: true);
            var code4 = new ParsedInstruction(4, operation, -1, IsRelative: true);

            var result = CodeValidator.Validate([], [code4, code2, code3, code1]);

            result.Errors.Where(error => error.Error is Error.CodeBranchTargetOutsideMemoryLimits)
                .Select(error => error.LineNumber).ToArray().ShouldBeSubsetOf(new[] { code1.LineNumber, code2.LineNumber });
        }

        [Fact]
        public void Validate_OperationReferencesLabelAssignedToItself_ReturnsCodeBranchInvalidArgumentError()
        {
            var code = new ParsedInstruction(1, Operation.Call, TargetTextIdentifier: "y", InstructionLabel: "y");

            var result = CodeValidator.Validate([], [code]);

            result.Errors.ShouldContain(error => error.Error == Error.CodeBranchInvalidArgument);
        }

        [Fact]
        public void Validate_LabelDeclaredButNeverReferenced_ReturnsUnusedLabelWarning()
        {
            var code = new ParsedInstruction(1, Operation.Call, InstructionLabel: "y");

            var result = CodeValidator.Validate([], [code]);

            result.Warnings.ShouldContain(warning => warning.Warning == Warning.CodeUnusedLabel);
        }

        [Fact]
        public void Validate_CodeLabelUsesSameNameAsDataVariable_ReturnsCodeLabelSharesNameWithVariableWarning()
        {
            var data = new ParsedData(1, "y", 0, 1);
            var code = new ParsedInstruction(2, Operation.Add, InstructionLabel: "y");

            var result = CodeValidator.Validate([data], [code]);

            result.Warnings.ShouldContain(warning => warning.Warning == Warning.CodeLabelSharesNameWithVariable);
        }

        [Fact]
        public void Validate_PushAddressValueAndNoDataHasBeenDeclared_ReturnsCodeAddressDoesNotReferenceVariableSpaceWarning()
        {
            var code1 = new ParsedInstruction(1, Operation.PushAddress, 0);
            var code2 = new ParsedInstruction(2, Operation.PushAddress, 5);

            var result = CodeValidator.Validate([], [code2, code1]);

            result.Warnings.Where(warning => warning.Warning is Warning.CodeAddressDoesNotReferenceVariableSpace)
                .Select(warnings => warnings.LineNumber).ToArray().ShouldBeSubsetOf(new[] { code1.LineNumber, code2.LineNumber });
        }

        [Fact]
        public void Validate_PushAddressValueReferencesUnallocatedDataMemory_ReturnsCodeAddressDoesNotReferenceVariableSpaceWarning()
        {
            var data1 = new ParsedData(1, "x", 0, 10);
            var data2 = new ParsedData(2, "y", 20, 10);
            var code1 = new ParsedInstruction(3, Operation.PushAddress, 100);
            var code2 = new ParsedInstruction(4, Operation.PushAddress, 15);

            var result = CodeValidator.Validate([data2, data1], [code2, code1]);

            result.Warnings.Where(warning => warning.Warning is Warning.CodeAddressDoesNotReferenceVariableSpace)
                .Select(warnings => warnings.LineNumber).ToArray().ShouldBeSubsetOf(new[] { code1.LineNumber, code2.LineNumber });
        }

        [Fact]
        public void Validate_CodeDoesNotContainHaltInstruction_ReturnsNoHaltInstructionDeclaredWarning()
        {
            List<ParsedInstruction> parsedInstructions =
            [
                new(2, Operation.PushValue, Constants.Min8BitValue),
                new(4, Operation.PushValue, Constants.Max8BitValue),
                new(5, Operation.PushValue, 3)
            ];

            var result = CodeValidator.Validate([], parsedInstructions);

            result.Warnings.ShouldContain(warning => warning.Warning == Warning.CodeNoHaltInstructionDeclared);
        }
    }
}
