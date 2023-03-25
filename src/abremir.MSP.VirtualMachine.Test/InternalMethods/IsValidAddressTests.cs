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

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(Constants.MemoryCapacity)]
        public void IsValidAddress_Invalid_ReturnsFalse(int address)
        {
            var result = VirtualMachine.IsValidAddress(null, address);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(Constants.MemoryCapacity)]
        public void IsValidAddress_Invalid_SetsStatusHaltsWithMemoryAddressViolation(int address)
        {
            VirtualMachine.Status.Should().NotBe(Status.Halted);

            VirtualMachine.IsValidAddress(null, address);

            VirtualMachine.Status.Should().Be(Status.Halted);
            VirtualMachine.HaltedBy.Should().Be(HaltReason.MemoryAddressViolation);
        }
    }
}
