using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class ResetIfHaltedTests : VirtualMachineTestsBase
    {
        [Fact]
        public void ResetIfHalted_StatusIsNotHalted_DoesNotReset()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.Stack.Should().NotBeEmpty();
            VirtualMachine.PC.Should().NotBe(0);

            var pc = VirtualMachine.PC;
            var stack = VirtualMachine.Stack;
            var haltedBy = VirtualMachine.HaltedBy;

            VirtualMachine.ResetIfHalted();

            VirtualMachine.PC.Should().Be(pc);
            VirtualMachine.Stack.Should().BeEquivalentTo(stack);
            VirtualMachine.HaltedBy.Should().Be(haltedBy);
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ClearsStack()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Status.Should().Be(Enums.Status.Halted);
            VirtualMachine.HaltedBy.Should().NotBeNull();
            VirtualMachine.Stack.Should().NotBeEmpty();

            VirtualMachine.ResetIfHalted();

            VirtualMachine.Stack.Should().BeEmpty();
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ResetsProgramCounter()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Status.Should().Be(Enums.Status.Halted);
            VirtualMachine.HaltedBy.Should().NotBeNull();
            VirtualMachine.PC.Should().NotBe(0);

            VirtualMachine.ResetIfHalted();

            VirtualMachine.PC.Should().Be(0);
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ClearsHaltedBy()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Status.Should().Be(Enums.Status.Halted);
            VirtualMachine.HaltedBy.Should().NotBeNull();

            VirtualMachine.ResetIfHalted();

            VirtualMachine.HaltedBy.Should().BeNull();
        }
    }
}
