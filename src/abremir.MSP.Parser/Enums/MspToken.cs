using abremir.MSP.Shared.Constants;
using Superpower.Display;

namespace abremir.MSP.Parser.Enums
{
    public enum MspToken
    {
        None,
        [Token(Category = "keyword", Example = Constants.DataSegment, Description = "Data Segment")]
        DataSegment,
        [Token(Category = "keyword", Example = "TAM")]
        Size,
        [Token(Category = "keyword", Example = "VAL")]
        Values,
        [Token(Category = "keyword", Example = Constants.CodeSegment, Description = "Code Segment")]
        CodeSegment,
        [Token(Category = "keyword", Example = "PUSH")]
        PushValue,
        [Token(Category = "keyword", Example = "PSHA")]
        PushAddress,
        [Token(Category = "keyword", Example = "LOAD")]
        LoadValue,
        [Token(Category = "keyword", Example = "LDA")]
        LoadAddress,
        [Token(Category = "keyword", Example = "STORE")]
        StoreValue,
        [Token(Category = "keyword", Example = "STRA")]
        StoreAddress,
        [Token(Category = "keyword", Example = "IN")]
        InputValue,
        [Token(Category = "keyword", Example = "OUT")]
        OutputValue,
        [Token(Category = "keyword", Example = "INC")]
        InputCharacter,
        [Token(Category = "keyword", Example = "OUTC")]
        OutputCharacter,
        [Token(Category = "keyword", Example = "ADD")]
        Add,
        [Token(Category = "keyword", Example = "SUB")]
        Subtract,
        [Token(Category = "keyword", Example = "MUL")]
        Multiply,
        [Token(Category = "keyword", Example = "DIV")]
        Divide,
        [Token(Category = "keyword", Example = "ADDA")]
        AddAddress,
        [Token(Category = "keyword", Example = "AND")]
        LogicAnd,
        [Token(Category = "keyword", Example = "OR")]
        LogicOr,
        [Token(Category = "keyword", Example = "NOT")]
        LogicNot,
        [Token(Category = "keyword", Example = "EQ")]
        Equal,
        [Token(Category = "keyword", Example = "NE")]
        NotEqual,
        [Token(Category = "keyword", Example = "LT")]
        LessThan,
        [Token(Category = "keyword", Example = "LE")]
        LessThanOrEqual,
        [Token(Category = "keyword", Example = "GT")]
        GreaterThan,
        [Token(Category = "keyword", Example = "GE")]
        GreaterThanOrEqual,
        [Token(Category = "keyword", Example = "ANDB")]
        BitwiseAnd,
        [Token(Category = "keyword", Example = "ORB")]
        BitwiseOr,
        [Token(Category = "keyword", Example = "NOTB")]
        BitwiseNot,
        [Token(Category = "keyword", Example = "JMP")]
        Jump,
        [Token(Category = "keyword", Example = "JMPF")]
        JumpIfFalse,
        [Token(Category = "keyword", Example = "CALL")]
        Call,
        [Token(Category = "keyword", Example = "RET")]
        ReturnFromCall,
        [Token(Category = "keyword", Example = "HALT")]
        Halt,
        [Token(Category = "keyword", Example = "NOOP")]
        NoOperation,
        [Token(Category = "text", Example = "a1_b")]
        Text,
        [Token(Category = "separator")]
        NewLine,
        [Token(Category = "text", Example = "123")]
        Number,
        [Token(Category = "text", Example = "+123 or -123")]
        SignedNumber,
        [Token(Category = "text", Example = ":")]
        Colon
    }
}
