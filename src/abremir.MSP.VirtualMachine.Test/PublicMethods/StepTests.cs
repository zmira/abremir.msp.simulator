using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    [TestClass]
    public class StepTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void Step_StatusInterrupted_RaisesInputRequestedEvent()
        {
            byte[] program = [(byte)Operation.InputValue];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            VirtualMachine.Step();

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void Step_StatusHalted_ClearsStack()
        {
            byte[] program = [(byte)Operation.PushValue, 1, (byte)Operation.Halt];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            Check.That(VirtualMachine.Stack.Where(value => value != 0)).Not.IsEmpty();

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

            Check.That(count).Is(1);
        }

        [TestMethod]
        public void Step_StatusHalted_SetsProgramCounterToZero()
        {
            byte[] program = [(byte)Operation.PushValue, 1, (byte)Operation.Halt];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            Check.That(VirtualMachine.PC).IsNotEqualTo((byte)0);

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

            Check.That(count).Is(1);
        }

        [TestMethod]
        public void Step_StatusHalted_SetsHaltedByToNull()
        {
            byte[] program = [(byte)Operation.PushValue, 1, (byte)Operation.Halt];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            Check.That(VirtualMachine.HaltedBy).IsNotNull();

            VirtualMachine.Step();

            Check.That(VirtualMachine.HaltedBy).IsNull();
        }

        [TestMethod]
        public void Step_SetsModeToStep()
        {
            byte[] program = [(byte)Operation.PushValue, 1];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            Check.That(VirtualMachine.Mode).Is(Mode.None);

            VirtualMachine.Step();

            Check.That(VirtualMachine.Mode).Is(Mode.Step);
        }

        [TestMethod]
        public void Step_SetsStatusToRunning()
        {
            byte[] program = [(byte)Operation.PushValue, 1];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            Check.That(VirtualMachine.Status).Is(Status.None);

            VirtualMachine.Step();

            Check.That(VirtualMachine.Status).Is(Status.Running);
        }

        [TestMethod]
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
