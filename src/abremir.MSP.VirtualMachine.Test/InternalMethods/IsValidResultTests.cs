using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class IsValidResultTests : VirtualMachineTestsBase
    {
        [Fact]
        public void IsValidResult_ValidNonArithmetic_ReturnsTrue()
        {
            var value = Random.Next(Constants.Min8BitValue, Constants.Max8BitValue);

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value);

            result.ShouldBeTrue();
        }

        [Fact]
        public void IsValidResult_ValidArithmetic_ReturnsTrue()
        {
            var value = Random.Next(Constants.Min8BitValue, sbyte.MaxValue);

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            result.ShouldBeTrue();
        }

        [Fact]
        public void IsValidResult_NonArithmeticLessThanMinimum_ReturnsFalse()
        {
            const int value = Constants.Min8BitValue - 1;

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value);

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsValidResult_NonArithmeticLessThanMinimum_HaltsWithUnderflowError()
        {
            const int value = Constants.Min8BitValue - 1;

            VirtualMachine.Status.ShouldNotBe(Status.Halted);

            VirtualMachine.IsValidResult(Operation.NoOperation, value);

            VirtualMachine.Status.ShouldBe(Status.Halted);
            VirtualMachine.HaltedBy.ShouldBe(HaltReason.UnderflowError);
        }

        [Fact]
        public void IsValidResult_ArithmeticLessThanMinimum_ReturnsFalse()
        {
            const int value = Constants.Min8BitValue - 1;

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsValidResult_ArithmeticLessThanMinimum_HaltsWithUnderflowError()
        {
            const int value = Constants.Min8BitValue - 1;

            VirtualMachine.Status.ShouldNotBe(Status.Halted);

            VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            VirtualMachine.Status.ShouldBe(Status.Halted);
            VirtualMachine.HaltedBy.ShouldBe(HaltReason.UnderflowError);
        }

        [Fact]
        public void IsValidResult_NonArithmeticGreaterThanMaximum_ReturnsFalse()
        {
            const int value = Constants.Max8BitValue + 1;

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value);

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsValidResult_NonArithmeticGreaterThanMaximum_HaltsWithOverflowError()
        {
            const int value = Constants.Max8BitValue + 1;

            VirtualMachine.Status.ShouldNotBe(Status.Halted);

            VirtualMachine.IsValidResult(Operation.NoOperation, value);

            VirtualMachine.Status.ShouldBe(Status.Halted);
            VirtualMachine.HaltedBy.ShouldBe(HaltReason.OverflowError);
        }

        [Fact]
        public void IsValidResult_ArithmeticGreaterThanMaximum_ReturnsFalse()
        {
            const int value = sbyte.MaxValue + 1;

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsValidResult_ArithmeticGreaterThanMaximum_HaltsWithOverflowError()
        {
            const int value = sbyte.MaxValue + 1;

            VirtualMachine.Status.ShouldNotBe(Status.Halted);

            VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            VirtualMachine.Status.ShouldBe(Status.Halted);
            VirtualMachine.HaltedBy.ShouldBe(HaltReason.OverflowError);
        }
    }
}
