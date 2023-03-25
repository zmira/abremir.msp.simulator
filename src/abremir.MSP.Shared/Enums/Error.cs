using System.ComponentModel;

namespace abremir.MSP.Shared.Enums
{
    public enum Error
    {
        [Description("Data Memory, or Program Memory not available")]
        DataMemoryOrProgramMemoryNotAvailable = 0,
        [Description("Expected MEMORIA DE DADOS")]
        DataSegmentExpected = 1,
        [Description("Data Memory exhausted")]
        DataMemoryCapacityExhausted = 2,
        [Description("Invalid Data Memory address")]
        DataInvalidAddress = 3,
        [Description("Data Memory identifier already defined")]
        DataRedefinitionOfVariable = 4,
        [Description("Unrecognized address of Data Memory")]
        DataUnrecognizedAddress = 5,
        [Description("Expected TAMANHO")]
        DataSizeTokenExpected = 6,
        [Description("Expected value for TAMANHO")]
        DataSizeExpected = 7,
        [Description("Invalid value for TAMANHO")]
        DataInvalidSize = 8,
        [Description("Expected initialization value(s) to be in [-128, 255]")]
        DataUnexpectedInitializationValues = 9,
        [Description("Initialization values exceed reserved memory")]
        DataInitializedValuesExceedReservedSpace = 10,
        [Description("8-bit integer value outside [-128, 255] range")]
        DataInitializationValueOutsideAllowedRange = 11,
        [Description("Unknown or unexpected token")]
        UnknownToken = 12,
        [Description("Expected CODIGO, or invalid variable identifier")]
        CodeSegmentExpectedOrInvalidToken = 13,
        [Description("End-of-line not found, or instruction expected")]
        EndOfLineNotFound = 14,
        [Description("Program Memory exhausted")]
        ProgramMemoryCapacityExhausted = 15,
        [Description("Label already defined")]
        CodeRedefinitionOfLabel = 16,
        [Description("Potential label without colon (:)")]
        CodePotentialLabelWithoutColon = 17,
        [Description("Unexpected instruction argument")]
        CodeUnexpectedArgument = 18,
        [Description("PUSH 8-bit integer argument outside [-128, 255] range")]
        CodePushArgumentOutsideAllowedRange = 19,
        [Description("Invalid PSHA address, 16-bit value must be in [0, 31999]")]
        CodePshaInvalidAddress = 20,
        [Description("Use of undeclared Data Memory identifier as PSHA argument")]
        CodePshaVariableNotDeclared = 21,
        [Description("Use of label, instead of Data Memory identifier, as PSHA argument")]
        CodePshaInvalidVariableArgument = 22,
        [Description("Unknown Data Memory identifier used as PSHA argument")]
        CodePshaInvalidArgument = 23,
        [Description("Undefined label")]
        CodeBranchUndefinedLabel = 24,
        [Description("Attempt to jump outside Program Memory limits ([0, 31999])")]
        CodeBranchTargetOutsideMemoryLimits = 25,
        [Description("Invalid argument for branch instruction (JMP, JMPF, CALL)")]
        CodeBranchInvalidArgument = 26,
        [Description("No code to assemble")]
        NoSourceDetectedToAssemble = 27,
        [Description("Syntax Error")]
        SyntaxError = 999,
        [Description("Other Exception")]
        Exception = 1000
    }
}
