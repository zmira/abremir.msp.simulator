using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class SetProgramCounterTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void SetProgramCounter_AddressIsNotValid_DoesNotUpdateProgramCounter()
        {
            var pc = VirtualMachine.PC;

            VirtualMachine.SetProgramCounter(Constants.MemoryCapacity);

            Check.That(VirtualMachine.PC).Is(pc);
        }

        [TestMethod]
        public void SetProgramCounter_AddressIsNotValid_ReturnsFalse()
        {
            var result = VirtualMachine.SetProgramCounter(Constants.MemoryCapacity);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void SetProgramCounter_AddressIsValid_UpdatesProgramCounter()
        {
            var newPc = (ushort)Random.Next(1, Constants.MemoryCapacity - 1);

            VirtualMachine.SetProgramCounter(newPc);

            Check.That(VirtualMachine.PC).Is(newPc);
        }

        [TestMethod]
        public void SetProgramCounter_AddressIsValid_RaisesProgramCounterUpdatedEvent()
        {
            var newPc = (ushort)Random.Next(1, Constants.MemoryCapacity - 1);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<ushort>((virtualMachine, handler) => virtualMachine.ProgramCounterUpdated += handler);

            VirtualMachine.SetProgramCounter(newPc);

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void SetProgramCounter_AddressIsValid_ReturnsTrue()
        {
            var newPc = (ushort)Random.Next(1, Constants.MemoryCapacity - 1);

            var result = VirtualMachine.SetProgramCounter(newPc);

            Check.That(result).IsTrue();
        }
    }
}
