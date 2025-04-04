using abremir.MSP.Shared.Enums;

namespace abremir.MSP.Shared.Test.Extensions
{
    [TestClass]
    public class OperationExtensionsTests
    {
        [TestMethod]
        [DataRow(Operation.NoOperation, 1)]
        [DataRow(Operation.PushValue, 2)]
        [DataRow(Operation.PushAddress, 3)]
        [DataRow(Operation.LoadValue, 1)]
        [DataRow(Operation.LoadAddress, 1)]
        [DataRow(Operation.StoreValue, 1)]
        [DataRow(Operation.StoreAddress, 1)]
        [DataRow(Operation.InputValue, 1)]
        [DataRow(Operation.OutputValue, 1)]
        [DataRow(Operation.Add, 1)]
        [DataRow(Operation.Subtract, 1)]
        [DataRow(Operation.Multiply, 1)]
        [DataRow(Operation.Divide, 1)]
        [DataRow(Operation.AddAddress, 1)]
        [DataRow(Operation.LogicAnd, 1)]
        [DataRow(Operation.LogicOr, 1)]
        [DataRow(Operation.LogicNot, 1)]
        [DataRow(Operation.Equal, 1)]
        [DataRow(Operation.NotEqual, 1)]
        [DataRow(Operation.LessThan, 1)]
        [DataRow(Operation.LessThanOrEqual, 1)]
        [DataRow(Operation.GreaterThan, 1)]
        [DataRow(Operation.GreaterThanOrEqual, 1)]
        [DataRow(Operation.Jump, 3)]
        [DataRow(Operation.JumpIfFalse, 3)]
        [DataRow(Operation.Call, 3)]
        [DataRow(Operation.ReturnFromCall, 1)]
        [DataRow(Operation.Halt, 1)]
        [DataRow(Operation.InputCharacter, 1)]
        [DataRow(Operation.OutputCharacter, 1)]
        [DataRow(Operation.BitwiseAnd, 1)]
        [DataRow(Operation.BitwiseOr, 1)]
        [DataRow(Operation.BitwiseNot, 1)]
        public void GetNumberOfMemoryCellsOccupied_ReturnsExpectedNumberOfOccupiedMemoryCellsForOperation(Operation operation, int expectedNumberOfCells)
        {
            var result = operation.GetNumberOfMemoryCellsOccupied();

            Check.That(expectedNumberOfCells).Is(result);
        }
    }
}
