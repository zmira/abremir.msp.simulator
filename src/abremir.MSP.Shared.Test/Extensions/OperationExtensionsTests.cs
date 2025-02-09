using abremir.MSP.Shared.Enums;

namespace abremir.MSP.Shared.Test.Extensions
{
    public class OperationExtensionsTests
    {
        [Theory]
        [InlineData(Operation.NoOperation, 1)]
        [InlineData(Operation.PushValue, 2)]
        [InlineData(Operation.PushAddress, 3)]
        [InlineData(Operation.LoadValue, 1)]
        [InlineData(Operation.LoadAddress, 1)]
        [InlineData(Operation.StoreValue, 1)]
        [InlineData(Operation.StoreAddress, 1)]
        [InlineData(Operation.InputValue, 1)]
        [InlineData(Operation.OutputValue, 1)]
        [InlineData(Operation.Add, 1)]
        [InlineData(Operation.Subtract, 1)]
        [InlineData(Operation.Multiply, 1)]
        [InlineData(Operation.Divide, 1)]
        [InlineData(Operation.AddAddress, 1)]
        [InlineData(Operation.LogicAnd, 1)]
        [InlineData(Operation.LogicOr, 1)]
        [InlineData(Operation.LogicNot, 1)]
        [InlineData(Operation.Equal, 1)]
        [InlineData(Operation.NotEqual, 1)]
        [InlineData(Operation.LessThan, 1)]
        [InlineData(Operation.LessThanOrEqual, 1)]
        [InlineData(Operation.GreaterThan, 1)]
        [InlineData(Operation.GreaterThanOrEqual, 1)]
        [InlineData(Operation.Jump, 3)]
        [InlineData(Operation.JumpIfFalse, 3)]
        [InlineData(Operation.Call, 3)]
        [InlineData(Operation.ReturnFromCall, 1)]
        [InlineData(Operation.Halt, 1)]
        [InlineData(Operation.InputCharacter, 1)]
        [InlineData(Operation.OutputCharacter, 1)]
        [InlineData(Operation.BitwiseAnd, 1)]
        [InlineData(Operation.BitwiseOr, 1)]
        [InlineData(Operation.BitwiseNot, 1)]
        public void GetNumberOfMemoryCellsOccupied_ReturnsExpectedNumberOfOccupiedMemoryCellsForOperation(Operation operation, int expectedNumberOfCells)
        {
            var result = operation.GetNumberOfMemoryCellsOccupied();

            Check.That(expectedNumberOfCells).Is(result);
        }
    }
}
