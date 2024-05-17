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
#pragma warning disable IDE0230 // Use UTF-8 string literal
            var data = new byte[] { 111 };
#pragma warning restore IDE0230 // Use UTF-8 string literal
            var program = new byte[] { (byte)Operation.PushValue, 55 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).WithData(data).Build();
            VirtualMachine.Step();
        }
        [Fact]
        public void Reset_SetsModeNone()
        {
            VirtualMachine.Mode.Should().NotBe(Mode.None);

            VirtualMachine.Reset();

            VirtualMachine.Mode.Should().Be(Mode.None);
        }

        [Fact]
        public void Reset_SetsStatusNone()
        {
            VirtualMachine.Status.Should().NotBe(Status.None);

            VirtualMachine.Reset();

            VirtualMachine.Status.Should().Be(Status.None);
        }

        [Fact]
        public void Reset_SetsHaltedByToNull()
        {
            var program = new byte[] { (byte)Operation.Halt };

            VirtualMachine.Reset();
            VirtualMachine.SetMemory(Array.Empty<byte>(), program);
            VirtualMachine.Step();

            VirtualMachine.HaltedBy.Should().NotBeNull();

            VirtualMachine.Reset();

            VirtualMachine.HaltedBy.Should().BeNull();
        }

        [Fact]
        public void Reset_SetsInterruptedByToNull()
        {
            var program = new byte[] { (byte)Operation.InputCharacter };

            VirtualMachine.Reset();
            VirtualMachine.SetMemory(Array.Empty<byte>(), program);
            VirtualMachine.Step();

            VirtualMachine.InterruptedBy.Should().NotBeNull();

            VirtualMachine.Reset();

            VirtualMachine.InterruptedBy.Should().BeNull();
        }

        [Fact]
        public void Reset_ClearsStack()
        {
            VirtualMachine.Stack.Where(value => value != 0).Should().NotBeEmpty();

            VirtualMachine.Reset();

            VirtualMachine.Stack.Where(value => value != 0).Should().BeEmpty();
        }

        [Fact]
        public void Reset_SetsProgramCounterToZero()
        {
            VirtualMachine.PC.Should().NotBe(0);

            VirtualMachine.Reset();

            VirtualMachine.PC.Should().Be(0);
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
            VirtualMachine.Data.Where(value => value != 0).Should().NotBeEmpty();

            VirtualMachine.Reset(true);

            VirtualMachine.Data.Where(value => value != 0).Should().BeEmpty();
        }

        [Fact]
        public void Reset_ClearAllMemoryTrue_ClearsProgramMemory()
        {
            VirtualMachine.Program.Where(value => value != 0).Should().NotBeEmpty();

            VirtualMachine.Reset(true);

            VirtualMachine.Program.Where(value => value != 0).Should().BeEmpty();
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
