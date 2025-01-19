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

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(Constants.MemoryCapacity)]
        public void IsValidAddress_Invalid_ReturnsFalse(int address)
        {
            var result = VirtualMachine.IsValidAddress(null, address);

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(Constants.MemoryCapacity)]
        public void IsValidAddress_Invalid_SetsStatusHaltsWithMemoryAddressViolation(int address)
        {
            VirtualMachine.Status.ShouldNotBe(Status.Halted);

            VirtualMachine.IsValidAddress(null, address);

            VirtualMachine.Status.ShouldBe(Status.Halted);
            VirtualMachine.HaltedBy.ShouldBe(HaltReason.MemoryAddressViolation);
        }
    }
}
