using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class InternalRunTests : VirtualMachineTestsBase
    {
        [Fact]
        public void InternalRun_StatusInterrupted_RaisesInputRequestedEvent()
        {
            byte[] program = [(byte)Operation.InputValue];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            VirtualMachine.Run();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void InternalRun_StatusHalted_ClearsStack()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

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

            VirtualMachine.Run();

            count.ShouldBe(1);
        }

        [Fact]
        public void InternalRun_StatusHalted_SetsProgramCounterToZero()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.PC.ShouldNotBe((ushort)0);

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

            VirtualMachine.Run();

            count.ShouldBe(1);
        }

        [Fact]
        public void InternalRun_StatusHalted_SetsHaltedByToNull()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.HaltedBy.ShouldNotBeNull();

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler)
                .Verify(eventArgs =>
                {
                    if (eventArgs.NewStatus == Enums.Status.Running
                        && VirtualMachine.HaltedBy is null)
                    {
                        count++;
                    }
                })
                .Build();

            VirtualMachine.Run();

            count.ShouldBe(1);
        }

        [Fact]
        public void InternalRun_SetsModeToRun()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<ModeChangedEventArgs>((virtualMachine, handler) => virtualMachine.ModeChanged += handler)
                .Verify(eventArgs =>
                {
                    if (eventArgs.NewMode == Enums.Mode.Run)
                    {
                        count++;
                    }
                })
                .Build();

            VirtualMachine.Run();

            count.ShouldBe(1);
        }

        [Fact]
        public void InternalRun_SetsStatusToRunning()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler)
                .Verify(eventArgs =>
                {
                    if (eventArgs.NewStatus == Enums.Status.Running)
                    {
                        count++;
                    }
                })
                .Build();

            VirtualMachine.Run();

            count.ShouldBe(1);
        }

        [Fact]
        public void InternalRun_ExecutesNextInstruction()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler)
                .Verify(_ => count++)
                .Build();

            VirtualMachine.Run();

            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void InternalRun_AfterStatusHalt_DoesNotExecuteNextInstruction()
        {
            var program = new byte[] { (byte)Operation.Halt, (byte)Operation.PushValue, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler)
                .Verify(_ => count++)
                .Build();

            VirtualMachine.Run();

            count.ShouldBe(1);
        }

        [Fact]
        public void InternalRun_AfterStatusInterrupted_DoesNotExecuteNextInstruction()
        {
            var program = new byte[] { (byte)Operation.InputValue, (byte)Operation.PushValue, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler)
                .Verify(_ => count++)
                .Build();

            VirtualMachine.Run();

            count.ShouldBe(1);
        }

        [Fact]
        public void InternalRun_AfterStatusSuspended_DoesNotExecuteNextInstruction()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.SetInstructionExecutionDuration(TimeSpan.FromSeconds(1));

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler)
                .Verify(_ => count++)
                .Build();

            VirtualMachine.Run();
            VirtualMachine.Suspend();

            count.ShouldBe(0);
        }

        [Fact]
        public void InternalRun_AfterModeStep_DoesNotExecuteNextInstruction()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.SetInstructionExecutionDuration(TimeSpan.FromSeconds(1));

            var count = 0;

            var hook = EventHook.For(VirtualMachine)
                .Hook<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler)
                .Verify(_ => count++)
                .Build();

            VirtualMachine.Run();
            VirtualMachine.SetMode(Enums.Mode.Step);

            count.ShouldBe(0);
        }
    }
}
