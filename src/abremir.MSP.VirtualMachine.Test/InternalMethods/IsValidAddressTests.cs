using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class IsValidAddressTests : VirtualMachineTestsBase
    {
        [Fact]
        public void IsValidAddress_Valid_ReturnsTrue()
        {
            var address = Random.Next(0, Constants.MemoryCapacity - 1);

            var result = VirtualMachine.IsValidAddress(null, address);

            Check.That(result).IsTrue();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(Constants.MemoryCapacity)]
        public void IsValidAddress_Invalid_ReturnsFalse(int address)
        {
            var result = VirtualMachine.IsValidAddress(null, address);

            Check.That(result).IsFalse();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(Constants.MemoryCapacity)]
        public void IsValidAddress_Invalid_SetsStatusHaltsWithMemoryAddressViolation(int address)
        {
            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.Halted);

            VirtualMachine.IsValidAddress(null, address);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.MemoryAddressViolation);
        }
    }
}
