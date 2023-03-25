using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class StepTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Step_StatusInterrupted_RaisesInputRequestedEvent()
        {
            var program = new[] { (byte)Operation.InputValue };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            VirtualMachine.Step();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void Step_StatusHalted_ClearsStack()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Stack.Where(value => value != 0).Should().NotBeEmpty();

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<ushort>((virtualMachine, handler) => virtualMachine.StackPointerUpdated += handler)
                .Verify(eventArgs =>
                {
                    if (eventArgs == Constants.MemoryCapacity - 1)
                    {
                        count++;
                    }
                })
                .Build();

            VirtualMachine.Step();

            count.Should().Be(1);
        }

        [Fact]
        public void Step_StatusHalted_SetsProgramCounterToZero()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.PC.Should().NotBe(0);

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<ushort>((virtualMachine, handler) => virtualMachine.ProgramCounterUpdated += handler)
                .Verify(eventArgs =>
                {
                    if (eventArgs == 0)
                    {
                        count++;
                    }
                })
                .Build();

            VirtualMachine.Step();

            count.Should().Be(1);
        }

        [Fact]
        public void Step_StatusHalted_SetsHaltedByToNull()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.HaltedBy.Should().NotBeNull();

            VirtualMachine.Step();

            VirtualMachine.HaltedBy.Should().BeNull();
        }

        [Fact]
        public void Step_SetsModeToStep()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            VirtualMachine.Mode.Should().Be(Mode.None);

            VirtualMachine.Step();

            VirtualMachine.Mode.Should().Be(Mode.Step);
        }

        [Fact]
        public void Step_SetsStatusToRunning()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            VirtualMachine.Status.Should().Be(Status.None);

            VirtualMachine.Step();

            VirtualMachine.Status.Should().Be(Status.Running);
        }

        [Fact]
        public void Step_ExecutesNextInstruction()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.Step();

            hook.Verify(Called.Once());
        }
    }
}
