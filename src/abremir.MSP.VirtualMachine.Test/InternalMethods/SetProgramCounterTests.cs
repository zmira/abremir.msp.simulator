using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class SetProgramCounterTests : VirtualMachineTestsBase
    {
        [Fact]
        public void SetProgramCounter_AddressIsNotValid_DoesNotUpdateProgramCounter()
        {
            var pc = VirtualMachine.PC;

            VirtualMachine.SetProgramCounter(Constants.MemoryCapacity);

            VirtualMachine.PC.ShouldBe(pc);
        }

        [Fact]
        public void SetProgramCounter_AddressIsNotValid_ReturnsFalse()
        {
            var result = VirtualMachine.SetProgramCounter(Constants.MemoryCapacity);

            result.ShouldBeFalse();
        }

        [Fact]
        public void SetProgramCounter_AddressIsValid_UpdatesProgramCounter()
        {
            var newPc = (ushort)Random.Next(1, Constants.MemoryCapacity - 1);

            VirtualMachine.SetProgramCounter(newPc);

            VirtualMachine.PC.ShouldBe(newPc);
        }

        [Fact]
        public void SetProgramCounter_AddressIsValid_RaisesProgramCounterUpdatedEvent()
        {
            var newPc = (ushort)Random.Next(1, Constants.MemoryCapacity - 1);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<ushort>((virtualMachine, handler) => virtualMachine.ProgramCounterUpdated += handler);

            VirtualMachine.SetProgramCounter(newPc);

            hook.Verify(Called.Once());
        }

        [Fact]
        public void SetProgramCounter_AddressIsValid_ReturnsTrue()
        {
            var newPc = (ushort)Random.Next(1, Constants.MemoryCapacity - 1);

            var result = VirtualMachine.SetProgramCounter(newPc);

            result.ShouldBeTrue();
        }
    }
}
