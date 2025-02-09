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

            Check.That(VirtualMachine.Stack).Not.IsEmpty();
            Check.That(VirtualMachine.PC).IsNotEqualTo((ushort)0);

            var pc = VirtualMachine.PC;
            var stack = VirtualMachine.Stack;
            var haltedBy = VirtualMachine.HaltedBy;

            VirtualMachine.ResetIfHalted();

            Check.That(VirtualMachine.PC).Is(pc);
            Check.That(VirtualMachine.Stack).Is(stack);
            Check.That(VirtualMachine.HaltedBy).Is(haltedBy);
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ClearsStack()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            Check.That(VirtualMachine.Status).Is(Enums.Status.Halted);
            Check.That(VirtualMachine.HaltedBy).IsNotNull();
            Check.That(VirtualMachine.Stack).Not.IsEmpty();

            VirtualMachine.ResetIfHalted();

            Check.That(VirtualMachine.Stack).IsEmpty();
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ResetsProgramCounter()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            Check.That(VirtualMachine.Status).Is(Enums.Status.Halted);
            Check.That(VirtualMachine.HaltedBy).IsNotNull();
            Check.That(VirtualMachine.PC).IsNotEqualTo((ushort)0);

            VirtualMachine.ResetIfHalted();

            Check.That(VirtualMachine.PC).Is((ushort)0);
        }

        [Fact]
        public void ResetIfHalted_StatusIsHalted_ClearsHaltedBy()
        {
            var program = new byte[] { (byte)Operation.PushValue, 5, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            Check.That(VirtualMachine.Status).Is(Enums.Status.Halted);
            Check.That(VirtualMachine.HaltedBy).IsNotNull();

            VirtualMachine.ResetIfHalted();

            Check.That(VirtualMachine.HaltedBy).IsNull();
        }
    }
}
