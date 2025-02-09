using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class IsValidResultTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void IsValidResult_ValidNonArithmetic_ReturnsTrue()
        {
            var value = Random.Next(Constants.Min8BitValue, Constants.Max8BitValue);

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void IsValidResult_ValidArithmetic_ReturnsTrue()
        {
            var value = Random.Next(Constants.Min8BitValue, sbyte.MaxValue);

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void IsValidResult_NonArithmeticLessThanMinimum_ReturnsFalse()
        {
            const int value = Constants.Min8BitValue - 1;

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void IsValidResult_NonArithmeticLessThanMinimum_HaltsWithUnderflowError()
        {
            const int value = Constants.Min8BitValue - 1;

            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.Halted);

            VirtualMachine.IsValidResult(Operation.NoOperation, value);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.UnderflowError);
        }

        [TestMethod]
        public void IsValidResult_ArithmeticLessThanMinimum_ReturnsFalse()
        {
            const int value = Constants.Min8BitValue - 1;

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void IsValidResult_ArithmeticLessThanMinimum_HaltsWithUnderflowError()
        {
            const int value = Constants.Min8BitValue - 1;

            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.Halted);

            VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.UnderflowError);
        }

        [TestMethod]
        public void IsValidResult_NonArithmeticGreaterThanMaximum_ReturnsFalse()
        {
            const int value = Constants.Max8BitValue + 1;

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void IsValidResult_NonArithmeticGreaterThanMaximum_HaltsWithOverflowError()
        {
            const int value = Constants.Max8BitValue + 1;

            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.Halted);

            VirtualMachine.IsValidResult(Operation.NoOperation, value);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.OverflowError);
        }

        [TestMethod]
        public void IsValidResult_ArithmeticGreaterThanMaximum_ReturnsFalse()
        {
            const int value = sbyte.MaxValue + 1;

            var result = VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void IsValidResult_ArithmeticGreaterThanMaximum_HaltsWithOverflowError()
        {
            const int value = sbyte.MaxValue + 1;

            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.Halted);

            VirtualMachine.IsValidResult(Operation.NoOperation, value, true);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.OverflowError);
        }
    }
}
