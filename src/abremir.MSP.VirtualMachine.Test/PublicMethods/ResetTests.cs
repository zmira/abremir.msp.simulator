using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class ResetTests : VirtualMachineTestsBase
    {
        public ResetTests()
        {
            byte[] data = [111];
            byte[] program = [(byte)Operation.PushValue, 55];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).WithData(data).Build();
            VirtualMachine.Step();
        }
        [Fact]
        public void Reset_SetsModeNone()
        {
            VirtualMachine.Mode.ShouldNotBe(Mode.None);

            VirtualMachine.Reset();

            VirtualMachine.Mode.ShouldBe(Mode.None);
        }

        [Fact]
        public void Reset_SetsStatusNone()
        {
            VirtualMachine.Status.ShouldNotBe(Status.None);

            VirtualMachine.Reset();

            VirtualMachine.Status.ShouldBe(Status.None);
        }

        [Fact]
        public void Reset_SetsHaltedByToNull()
        {
            var program = new byte[] { (byte)Operation.Halt };

            VirtualMachine.Reset();
            VirtualMachine.SetMemory([], program);
            VirtualMachine.Step();

            VirtualMachine.HaltedBy.ShouldNotBeNull();

            VirtualMachine.Reset();

            VirtualMachine.HaltedBy.ShouldBeNull();
        }

        [Fact]
        public void Reset_SetsInterruptedByToNull()
        {
            var program = new byte[] { (byte)Operation.InputCharacter };

            VirtualMachine.Reset();
            VirtualMachine.SetMemory([], program);
            VirtualMachine.Step();

            VirtualMachine.InterruptedBy.ShouldNotBeNull();

            VirtualMachine.Reset();

            VirtualMachine.InterruptedBy.ShouldBeNull();
        }

        [Fact]
        public void Reset_ClearsStack()
        {
            VirtualMachine.Stack.Where(value => value != 0).ShouldNotBeEmpty();

            VirtualMachine.Reset();

            VirtualMachine.Stack.Where(value => value != 0).ShouldBeEmpty();
        }

        [Fact]
        public void Reset_SetsProgramCounterToZero()
        {
            VirtualMachine.PC.ShouldNotBe((byte)0);

            VirtualMachine.Reset();

            VirtualMachine.PC.ShouldBe((byte)0);
        }

        [Fact]
        public void Reset_RaisesVirtualMachineResetEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .HookOnly((virtualMachine, handler) => virtualMachine.VirtualMachineReset += handler);

            VirtualMachine.Reset(true);

            hook.Verify(Called.Once());
        }

        [Fact]
        public void Reset_ClearAllMemoryTrue_ClearsDataMemory()
        {
            VirtualMachine.Data.Where(value => value != 0).ShouldNotBeEmpty();

            VirtualMachine.Reset(true);

            VirtualMachine.Data.Where(value => value != 0).ShouldBeEmpty();
        }

        [Fact]
        public void Reset_ClearAllMemoryTrue_ClearsProgramMemory()
        {
            VirtualMachine.Program.Where(value => value != 0).ShouldNotBeEmpty();

            VirtualMachine.Reset(true);

            VirtualMachine.Program.Where(value => value != 0).ShouldBeEmpty();
        }

        [Fact]
        public void Reset_ClearAllMemoryTrue_RaisesVirtualMachineMemorySetEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .HookOnly((virtualMachine, handler) => virtualMachine.VirtualMachineMemorySet += handler);

            VirtualMachine.Reset(true);

            hook.Verify(Called.Once());
        }
    }
}
