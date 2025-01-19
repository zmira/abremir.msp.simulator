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
            byte[] program = [(byte)Operation.InputValue];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            VirtualMachine.Step();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void Step_StatusHalted_ClearsStack()
        {
            byte[] program = [(byte)Operation.PushValue, 1, (byte)Operation.Halt];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Stack.Where(value => value != 0).ShouldNotBeEmpty();

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

            count.ShouldBe(1);
        }

        [Fact]
        public void Step_StatusHalted_SetsProgramCounterToZero()
        {
            byte[] program = [(byte)Operation.PushValue, 1, (byte)Operation.Halt];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.PC.ShouldNotBe((byte)0);

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

            count.ShouldBe(1);
        }

        [Fact]
        public void Step_StatusHalted_SetsHaltedByToNull()
        {
            byte[] program = [(byte)Operation.PushValue, 1, (byte)Operation.Halt];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.HaltedBy.ShouldNotBeNull();

            VirtualMachine.Step();

            VirtualMachine.HaltedBy.ShouldBeNull();
        }

        [Fact]
        public void Step_SetsModeToStep()
        {
            byte[] program = [(byte)Operation.PushValue, 1];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            VirtualMachine.Mode.ShouldBe(Mode.None);

            VirtualMachine.Step();

            VirtualMachine.Mode.ShouldBe(Mode.Step);
        }

        [Fact]
        public void Step_SetsStatusToRunning()
        {
            byte[] program = [(byte)Operation.PushValue, 1];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            VirtualMachine.Status.ShouldBe(Status.None);

            VirtualMachine.Step();

            VirtualMachine.Status.ShouldBe(Status.Running);
        }

        [Fact]
        public void Step_ExecutesNextInstruction()
        {
            byte[] program = [(byte)Operation.PushValue, 1];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.Step();

            hook.Verify(Called.Once());
        }
    }
}
