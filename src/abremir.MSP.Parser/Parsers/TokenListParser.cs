using abremir.MSP.Parser.Enums;
using abremir.MSP.Parser.Extensions;
using abremir.MSP.Parser.Models;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Models;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace abremir.MSP.Parser.Parsers
{
    public class TokenListParser : ITokenListParser
    {
        public TokenListParserResult<MspToken, TokenListParserValue> Parse(TokenList<MspToken> tokens)
        {
            return ParseTokens(tokens);
        }

        private static readonly TokenListParser<MspToken, int> Number =
            Token.EqualTo(MspToken.Number)
            .Apply(Numerics.IntegerInt32);

        private static readonly TokenListParser<MspToken, int> SignedNumber =
            Token.EqualTo(MspToken.SignedNumber)
            .Apply(Numerics.IntegerInt32);

        // MEMORIA DE DADOS
        private static readonly TokenListParser<MspToken, SegmentType> DataPragma =
            from _ in Token.EqualTo(MspToken.DataSegment)
            from __ in Token.EqualTo(MspToken.NewLine).Many()
            select SegmentType.Data;

        // Data declaration line
        private static readonly TokenListParser<MspToken, int[]> NumberValuesDeclaration =
            from _ in Token.EqualTo(MspToken.Values)
            from values in Number.Named(nameof(Error.DataUnexpectedInitializationValues)).AtLeastOnce()
            select values;

        private static readonly TokenListParser<MspToken, int[]> SignedNumberValuesDeclaration =
            from _ in Token.EqualTo(MspToken.Values)
            from values in SignedNumber.AtLeastOnce()
            select values;

        private static readonly TokenListParser<MspToken, int[]> ValuesDeclaration =
            NumberValuesDeclaration.Try()
            .Or(SignedNumberValuesDeclaration);

        private static readonly TokenListParser<MspToken, ParsedData> DataDeclarationLine =
            from variableName in Token.EqualTo(MspToken.Text)
            from address in Number.Named(nameof(Error.DataUnrecognizedAddress))
            from _ in Token.EqualTo(MspToken.Size).Named(nameof(Error.DataSizeTokenExpected))
            from size in Number.Named(nameof(Error.DataSizeExpected))
            from values in ValuesDeclaration!.OptionalOrDefault()
            from __ in Token.EqualTo(MspToken.NewLine).Many()
            select new ParsedData(variableName.Position.Line, variableName.ToStringValue(), address, size, values);

        // Data segment
        private static readonly TokenListParser<MspToken, ParsedData[]> DataSegment =
            from _ in DataPragma.AtLeastOnce().Named(nameof(Error.DataSegmentExpected))
            from dataLines in DataDeclarationLine.Many()
            from __ in Token.EqualTo(MspToken.NewLine).Many()
            select dataLines;

        // CODIGO
        private static readonly TokenListParser<MspToken, SegmentType> CodePragma =
            from _ in Token.EqualTo(MspToken.CodeSegment)
            from __ in Token.EqualTo(MspToken.NewLine).Many()
            select SegmentType.Code;

        // PUSH - unsigned number
        private static readonly TokenListParser<MspToken, Instruction> PushNumberInstruction =
            from keyword in Token.EqualTo(MspToken.PushValue)
            from value in Number.Named(nameof(Error.CodePushArgumentOutsideAllowedRange))
            select new Instruction(keyword.Position.Line, Operation.PushValue, NumericalValue: value);

        // PUSH - signed number
        private static readonly TokenListParser<MspToken, Instruction> PushSignedNumberInstruction =
            from keyword in Token.EqualTo(MspToken.PushValue)
            from value in SignedNumber
            select new Instruction(keyword.Position.Line, Operation.PushValue, NumericalValue: value);

        // PUSH
        private static readonly TokenListParser<MspToken, Instruction> PushInstruction =
            PushNumberInstruction.Try()
            .Or(PushSignedNumberInstruction);

        // PSHA - address
        private static readonly TokenListParser<MspToken, Instruction> PushAddressInstruction =
            from keyword in Token.EqualTo(MspToken.PushAddress)
            from address in Number
            select new Instruction(keyword.Position.Line, Operation.PushAddress, NumericalValue: address);

        // PSHA - variable name
        private static readonly TokenListParser<MspToken, Instruction> PushAddressVariableInstruction =
            from keyword in Token.EqualTo(MspToken.PushAddress)
            from variableName in Token.EqualTo(MspToken.Text)
            select new Instruction(keyword.Position.Line, Operation.PushAddress, TextIdentifier: variableName.ToStringValue());

        // LOAD
        private static readonly TokenListParser<MspToken, Instruction> LoadInstruction =
            from keyword in Token.EqualTo(MspToken.LoadValue)
            select new Instruction(keyword.Position.Line, Operation.LoadValue);

        // LDA
        private static readonly TokenListParser<MspToken, Instruction> LoadAddressInstruction =
            from keyword in Token.EqualTo(MspToken.LoadAddress)
            select new Instruction(keyword.Position.Line, Operation.LoadAddress);

        // STORE
        private static readonly TokenListParser<MspToken, Instruction> StoreInstruction =
            from keyword in Token.EqualTo(MspToken.StoreValue)
            select new Instruction(keyword.Position.Line, Operation.StoreValue);

        // STRA
        private static readonly TokenListParser<MspToken, Instruction> StoreAddressInstruction =
            from keyword in Token.EqualTo(MspToken.StoreAddress)
            select new Instruction(keyword.Position.Line, Operation.StoreAddress);

        // IN
        private static readonly TokenListParser<MspToken, Instruction> InputValueInstruction =
            from keyword in Token.EqualTo(MspToken.InputValue)
            select new Instruction(keyword.Position.Line, Operation.InputValue);

        // INC
        private static readonly TokenListParser<MspToken, Instruction> InputCharacterInstruction =
            from keyword in Token.EqualTo(MspToken.InputCharacter)
            select new Instruction(keyword.Position.Line, Operation.InputCharacter);

        // OUT
        private static readonly TokenListParser<MspToken, Instruction> OutputValueInstruction =
            from keyword in Token.EqualTo(MspToken.OutputValue)
            select new Instruction(keyword.Position.Line, Operation.OutputValue);

        // OUTC
        private static readonly TokenListParser<MspToken, Instruction> OutputCharacterInstruction =
            from keyword in Token.EqualTo(MspToken.OutputCharacter)
            select new Instruction(keyword.Position.Line, Operation.OutputCharacter);

        // ADD
        private static readonly TokenListParser<MspToken, Instruction> AddInstruction =
            from keyword in Token.EqualTo(MspToken.Add)
            select new Instruction(keyword.Position.Line, Operation.Add);

        // SUB
        private static readonly TokenListParser<MspToken, Instruction> SubtractInstruction =
            from keyword in Token.EqualTo(MspToken.Subtract)
            select new Instruction(keyword.Position.Line, Operation.Subtract);

        // MUL
        private static readonly TokenListParser<MspToken, Instruction> MultiplyInstruction =
            from keyword in Token.EqualTo(MspToken.Multiply)
            select new Instruction(keyword.Position.Line, Operation.Multiply);

        // DIV
        private static readonly TokenListParser<MspToken, Instruction> DivideInstruction =
            from keyword in Token.EqualTo(MspToken.Divide)
            select new Instruction(keyword.Position.Line, Operation.Divide);

        // ADDA
        private static readonly TokenListParser<MspToken, Instruction> AddAddressInstruction =
            from keyword in Token.EqualTo(MspToken.AddAddress)
            select new Instruction(keyword.Position.Line, Operation.AddAddress);

        // AND
        private static readonly TokenListParser<MspToken, Instruction> LogicAndInstruction =
            from keyword in Token.EqualTo(MspToken.LogicAnd)
            select new Instruction(keyword.Position.Line, Operation.LogicAnd);

        // OR
        private static readonly TokenListParser<MspToken, Instruction> LogicOrInstruction =
            from keyword in Token.EqualTo(MspToken.LogicOr)
            select new Instruction(keyword.Position.Line, Operation.LogicOr);

        // NOT
        private static readonly TokenListParser<MspToken, Instruction> LogicNotInstruction =
            from keyword in Token.EqualTo(MspToken.LogicNot)
            select new Instruction(keyword.Position.Line, Operation.LogicNot);

        // EQ
        private static readonly TokenListParser<MspToken, Instruction> EqualInstruction =
            from keyword in Token.EqualTo(MspToken.Equal)
            select new Instruction(keyword.Position.Line, Operation.Equal);

        // NE
        private static readonly TokenListParser<MspToken, Instruction> NotEqualInstruction =
            from keyword in Token.EqualTo(MspToken.NotEqual)
            select new Instruction(keyword.Position.Line, Operation.NotEqual);

        // LT
        private static readonly TokenListParser<MspToken, Instruction> LessThanInstruction =
            from keyword in Token.EqualTo(MspToken.LessThan)
            select new Instruction(keyword.Position.Line, Operation.LessThan);

        // LE
        private static readonly TokenListParser<MspToken, Instruction> LessThanOrEqualInstruction =
            from keyword in Token.EqualTo(MspToken.LessThanOrEqual)
            select new Instruction(keyword.Position.Line, Operation.LessThanOrEqual);

        // GT
        private static readonly TokenListParser<MspToken, Instruction> GreaterThanInstruction =
            from keyword in Token.EqualTo(MspToken.GreaterThan)
            select new Instruction(keyword.Position.Line, Operation.GreaterThan);

        // GE
        private static readonly TokenListParser<MspToken, Instruction> GreaterThanOrEqualInstruction =
            from keyword in Token.EqualTo(MspToken.GreaterThanOrEqual)
            select new Instruction(keyword.Position.Line, Operation.GreaterThanOrEqual);

        // ANDB
        private static readonly TokenListParser<MspToken, Instruction> BitwiseAndInstruction =
            from keyword in Token.EqualTo(MspToken.BitwiseAnd)
            select new Instruction(keyword.Position.Line, Operation.BitwiseAnd);

        // ORB
        private static readonly TokenListParser<MspToken, Instruction> BitwiseOrInstruction =
            from keyword in Token.EqualTo(MspToken.BitwiseOr)
            select new Instruction(keyword.Position.Line, Operation.BitwiseOr);

        // NOTB
        private static readonly TokenListParser<MspToken, Instruction> BitwiseNotInstruction =
            from keyword in Token.EqualTo(MspToken.BitwiseNot)
            select new Instruction(keyword.Position.Line, Operation.BitwiseNot);

        // JMP - address
        private static readonly TokenListParser<MspToken, Instruction> UnconditionalJumpToAddressInstruction =
            from keyword in Token.EqualTo(MspToken.Jump)
            from address in Number
            select new Instruction(keyword.Position.Line, Operation.Jump, NumericalValue: address);

        // JMP - label
        private static readonly TokenListParser<MspToken, Instruction> UnconditionalJumpToLabelInstruction =
            from keyword in Token.EqualTo(MspToken.Jump)
            from label in Token.EqualTo(MspToken.Text)
            select new Instruction(keyword.Position.Line, Operation.Jump, TextIdentifier: label.ToStringValue());

        // JMP - relative address
        private static readonly TokenListParser<MspToken, Instruction> UnconditionalJumpToRelativeAddressInstruction =
            from keyword in Token.EqualTo(MspToken.Jump)
            from relativeAddress in SignedNumber
            select new Instruction(keyword.Position.Line, Operation.Jump, NumericalValue: relativeAddress, IsRelative: true);

        // JMPF - address
        private static readonly TokenListParser<MspToken, Instruction> JumpIfFalseToAddressInstruction =
            from keyword in Token.EqualTo(MspToken.JumpIfFalse)
            from address in Number
            select new Instruction(keyword.Position.Line, Operation.JumpIfFalse, NumericalValue: address);

        // JMPF - label
        private static readonly TokenListParser<MspToken, Instruction> JumpIfFalseToLabelInstruction =
            from keyword in Token.EqualTo(MspToken.JumpIfFalse)
            from label in Token.EqualTo(MspToken.Text)
            select new Instruction(keyword.Position.Line, Operation.JumpIfFalse, TextIdentifier: label.ToStringValue());

        // JMPF - relative address
        private static readonly TokenListParser<MspToken, Instruction> JumpIfFalseToRelativeAddressInstruction =
            from keyword in Token.EqualTo(MspToken.JumpIfFalse)
            from relativeAddress in SignedNumber
            select new Instruction(keyword.Position.Line, Operation.JumpIfFalse, NumericalValue: relativeAddress, IsRelative: true);

        // CALL - address
        private static readonly TokenListParser<MspToken, Instruction> CallSubroutineAtAddressInstruction =
            from keyword in Token.EqualTo(MspToken.Call)
            from address in Number
            select new Instruction(keyword.Position.Line, Operation.Call, NumericalValue: address);

        // CALL - label
        private static readonly TokenListParser<MspToken, Instruction> CallSubroutineAtLabelInstruction =
            from keyword in Token.EqualTo(MspToken.Call)
            from label in Token.EqualTo(MspToken.Text)
            select new Instruction(keyword.Position.Line, Operation.Call, TextIdentifier: label.ToStringValue());

        // CALL - relative address
        private static readonly TokenListParser<MspToken, Instruction> CallSubroutineAtRelativeAddressInstruction =
            from keyword in Token.EqualTo(MspToken.Call)
            from relativeAddress in SignedNumber
            select new Instruction(keyword.Position.Line, Operation.Call, NumericalValue: relativeAddress, IsRelative: true);

        // RET
        private static readonly TokenListParser<MspToken, Instruction> ReturnFromCallInstruction =
            from keyword in Token.EqualTo(MspToken.ReturnFromCall)
            select new Instruction(keyword.Position.Line, Operation.ReturnFromCall);

        // HALT
        private static readonly TokenListParser<MspToken, Instruction> HaltInstruction =
            from keyword in Token.EqualTo(MspToken.Halt)
            select new Instruction(keyword.Position.Line, Operation.Halt);

        // NOOP
        private static readonly TokenListParser<MspToken, Instruction> NoOperationInstruction =
            from keyword in Token.EqualTo(MspToken.NoOperation)
            select new Instruction(keyword.Position.Line, Operation.NoOperation);

        // Instruction
        private static readonly TokenListParser<MspToken, Instruction> Instruction =
            from _ in Token.EqualTo(MspToken.NewLine).Many()
            from ins in
                // Push operations
                PushInstruction
                .Or(PushAddressInstruction.Try())
                .Or(PushAddressVariableInstruction)
                // Load operations
                .Or(LoadInstruction)
                .Or(LoadAddressInstruction)
                // Store operations
                .Or(StoreInstruction)
                .Or(StoreAddressInstruction)
                // Input/Output operations
                .Or(InputValueInstruction)
                .Or(InputCharacterInstruction)
                .Or(OutputValueInstruction)
                .Or(OutputCharacterInstruction)
                // Arithmetic operations
                .Or(AddInstruction)
                .Or(SubtractInstruction)
                .Or(MultiplyInstruction)
                .Or(DivideInstruction)
                .Or(AddAddressInstruction)
                // Logic operations
                .Or(LogicAndInstruction)
                .Or(LogicOrInstruction)
                .Or(LogicNotInstruction)
                // Bitwise operations
                .Or(BitwiseAndInstruction)
                .Or(BitwiseOrInstruction)
                .Or(BitwiseNotInstruction)
                // Comparison operations
                .Or(EqualInstruction)
                .Or(NotEqualInstruction)
                .Or(LessThanInstruction)
                .Or(LessThanOrEqualInstruction)
                .Or(GreaterThanInstruction)
                .Or(GreaterThanOrEqualInstruction)
                // Control operations
                .Or(UnconditionalJumpToAddressInstruction.Try())
                .Or(UnconditionalJumpToLabelInstruction.Try())
                .Or(UnconditionalJumpToRelativeAddressInstruction)
                .Or(JumpIfFalseToAddressInstruction.Try())
                .Or(JumpIfFalseToLabelInstruction.Try())
                .Or(JumpIfFalseToRelativeAddressInstruction)
                .Or(CallSubroutineAtAddressInstruction.Try())
                .Or(CallSubroutineAtLabelInstruction.Try())
                .Or(CallSubroutineAtRelativeAddressInstruction)
                .Or(ReturnFromCallInstruction)
                .Or(HaltInstruction)
                .Or(NoOperationInstruction)
            from __ in Token.EqualTo(MspToken.NewLine).Named(ins.IsUnaryOperation() ? nameof(Error.CodeUnexpectedArgument) : nameof(Error.EndOfLineNotFound))
            select ins;

        // Label
        private static readonly TokenListParser<MspToken, string> InstructionLabel =
            from _ in Token.EqualTo(MspToken.NewLine).Many()
            from text in Token.EqualTo(MspToken.Text)
            from colon in Token.EqualTo(MspToken.Colon).Named(nameof(Error.CodePotentialLabelWithoutColon))
            from __ in Token.EqualTo(MspToken.NewLine).Many()
            select text.ToStringValue();

        // Instruction line
        private static readonly TokenListParser<MspToken, ParsedInstruction> Line =
            from label in InstructionLabel!.OptionalOrDefault()
            from instruction in Instruction
            select new ParsedInstruction(instruction.LineNumber, instruction.Operation, instruction.NumericalValue, instruction.TextIdentifier, instruction.IsRelative, label);

        // Code segment
        private static readonly TokenListParser<MspToken, ParsedInstruction[]> CodeSegment =
            from _ in CodePragma.AtLeastOnce().Named(nameof(Error.CodeSegmentExpectedOrInvalidToken))
            from instructionLines in Line.Many()
            select instructionLines;

        // Parser
        private static readonly TokenListParser<MspToken, TokenListParserValue> ParseTokens =
            from _ in Token.EqualTo(MspToken.NewLine).Many()
            from data in DataSegment
            from instructions in CodeSegment.AtEnd()
            select new TokenListParserValue(new List<ParsedData>(data), new List<ParsedInstruction>(instructions));
    }
}
