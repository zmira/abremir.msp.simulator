using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    [TestClass]
    public class ResetTests : VirtualMachineTestsBase
    {
        public ResetTests()
        {
            byte[] data = [111];
            byte[] program = [(byte)Operation.PushValue, 55];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).WithData(data).Build();
            VirtualMachine.Step();
        }
        [TestMethod]
        public void Reset_SetsModeNone()
        {
            Check.That(VirtualMachine.Mode).IsNotEqualTo(Mode.None);

            VirtualMachine.Reset();

            Check.That(VirtualMachine.Mode).Is(Mode.None);
        }

        [TestMethod]
        public void Reset_SetsStatusNone()
        {
            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.None);

            VirtualMachine.Reset();

            Check.That(VirtualMachine.Status).Is(Status.None);
        }

        [TestMethod]
        public void Reset_SetsHaltedByToNull()
        {
            var program = new byte[] { (byte)Operation.Halt };

            VirtualMachine.Reset();
            VirtualMachine.SetMemory([], program);
            VirtualMachine.Step();

            Check.That(VirtualMachine.HaltedBy).IsNotNull();

            VirtualMachine.Reset();

            Check.That(VirtualMachine.HaltedBy).IsNull();
        }

        [TestMethod]
        public void Reset_SetsInterruptedByToNull()
        {
            var program = new byte[] { (byte)Operation.InputCharacter };

            VirtualMachine.Reset();
            VirtualMachine.SetMemory([], program);
            VirtualMachine.Step();

            Check.That(VirtualMachine.InterruptedBy).IsNotNull();

            VirtualMachine.Reset();

            Check.That(VirtualMachine.InterruptedBy).IsNull();
        }

        [TestMethod]
        public void Reset_ClearsStack()
        {
            Check.That(VirtualMachine.Stack.Where(value => value != 0)).Not.IsEmpty();

            VirtualMachine.Reset();

            Check.That(VirtualMachine.Stack.Where(value => value != 0)).IsEmpty();
        }

        [TestMethod]
        public void Reset_SetsProgramCounterToZero()
        {
            Check.That(VirtualMachine.PC).IsNotEqualTo((byte)0);

            VirtualMachine.Reset();

            Check.That(VirtualMachine.PC).Is((byte)0);
        }

        [TestMethod]
        public void Reset_RaisesVirtualMachineResetEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .HookOnly((virtualMachine, handler) => virtualMachine.VirtualMachineReset += handler);

            VirtualMachine.Reset(true);

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void Reset_ClearAllMemoryTrue_ClearsDataMemory()
        {
            Check.That(VirtualMachine.Data.Where(value => value != 0)).Not.IsEmpty();

            VirtualMachine.Reset(true);

            Check.That(VirtualMachine.Data.Where(value => value != 0)).IsEmpty();
        }

        [TestMethod]
        public void Reset_ClearAllMemoryTrue_ClearsProgramMemory()
        {
            Check.That(VirtualMachine.Program.Where(value => value != 0)).Not.IsEmpty();

            VirtualMachine.Reset(true);

            Check.That(VirtualMachine.Program.Where(value => value != 0)).IsEmpty();
        }

        [TestMethod]
        public void Reset_ClearAllMemoryTrue_RaisesVirtualMachineMemorySetEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .HookOnly((virtualMachine, handler) => virtualMachine.VirtualMachineMemorySet += handler);

            VirtualMachine.Reset(true);

            hook.Verify(Called.Once());
        }
    }
}
