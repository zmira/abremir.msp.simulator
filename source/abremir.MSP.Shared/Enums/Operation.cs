using System.ComponentModel;

namespace abremir.MSP.Shared.Enums
{
    public enum Operation
    {
        [Description("NOOP")]
        NoOperation = 0,
        [Description("PUSH")]
        PushValue = 1,
        [Description("PSHA")]
        PushAddress = 2,
        [Description("LOAD")]
        LoadValue = 3,
        [Description("LDA")]
        LoadAddress = 4,
        [Description("STORE")]
        StoreValue = 5,
        [Description("STRA")]
        StoreAddress = 6,
        [Description("IN")]
        InputValue = 7,
        [Description("OUT")]
        OutputValue = 8,
        [Description("ADD")]
        Add = 9,
        [Description("SUB")]
        Subtract = 10,
        [Description("MUL")]
        Multiply = 11,
        [Description("DIV")]
        Divide = 12,
        [Description("ADDA")]
        AddAddress = 13,
        [Description("AND")]
        LogicAnd = 14,
        [Description("OR")]
        LogicOr = 15,
        [Description("NOT")]
        LogicNot = 16,
        [Description("EQ")]
        Equal = 17,
        [Description("NE")]
        NotEqual = 18,
        [Description("LT")]
        LessThan = 19,
        [Description("LE")]
        LessThanOrEqual = 20,
        [Description("GT")]
        GreaterThan = 21,
        [Description("GE")]
        GreaterThanOrEqual = 22,
        [Description("JMP")]
        Jump = 23,
        [Description("JMPF")]
        JumpIfFalse = 24,
        [Description("CALL")]
        Call = 25,
        [Description("RET")]
        ReturnFromCall = 26,
        [Description("HALT")]
        Halt = 27,
        [Description("INC")]
        InputCharacter = 28,
        [Description("OUTC")]
        OutputCharacter = 29,
        [Description("ANDB")]
        BitwiseAnd = 30,
        [Description("ORB")]
        BitwiseOr = 31,
        [Description("NOTB")]
        BitwiseNot = 32
    }
}
