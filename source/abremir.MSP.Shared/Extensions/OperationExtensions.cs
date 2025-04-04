using abremir.MSP.Shared.Enums;

namespace abremir.MSP.Shared.Extensions
{
    public static class OperationExtensions
    {
        public static byte GetNumberOfMemoryCellsOccupied(this Operation operation)
        {
            return operation switch
            {
                Operation.PushValue => 2,
                Operation.PushAddress
                or Operation.Jump
                or Operation.JumpIfFalse
                or Operation.Call => 3,
                _ => 1
            };
        }
    }
}
