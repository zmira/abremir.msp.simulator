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

            VirtualMachine.Stack.ShouldNotBeEmpty();
            VirtualMachine.PC.ShouldNotBe((ushort)0);

            var pc = VirtualMachine.PC;
            var stack = VirtualMachine.Stack;
            var haltedBy = VirtualMachine.HaltedBy;

            VirtualMachine.ResetIfHalted();

            VirtualMachine.PC.ShouldBe(pc);
            VirtualMachine.Stack.ShouldBeEquivalentTo(stack);
            VirtualMachine.HaltedBy.ShouldBe(haltedBy);
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ClearsStack()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Status.ShouldBe(Enums.Status.Halted);
            VirtualMachine.HaltedBy.ShouldNotBeNull();
            VirtualMachine.Stack.ShouldNotBeEmpty();

            VirtualMachine.ResetIfHalted();

            VirtualMachine.Stack.ShouldBeEmpty();
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ResetsProgramCounter()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Status.ShouldBe(Enums.Status.Halted);
            VirtualMachine.HaltedBy.ShouldNotBeNull();
            VirtualMachine.PC.ShouldNotBe((ushort)0);

            VirtualMachine.ResetIfHalted();

            VirtualMachine.PC.ShouldBe((ushort)0);
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ClearsHaltedBy()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Status.ShouldBe(Enums.Status.Halted);
            VirtualMachine.HaltedBy.ShouldNotBeNull();

            VirtualMachine.ResetIfHalted();

            VirtualMachine.HaltedBy.ShouldBeNull();
        }
    }
}
