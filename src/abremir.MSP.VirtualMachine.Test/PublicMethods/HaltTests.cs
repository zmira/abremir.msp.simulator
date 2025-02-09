﻿using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class HaltTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Halt_HaltsExecutionWithForceHaltReason()
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler)
                .Verify(eventArgs => Check.That(eventArgs.Reason).Is(Enums.HaltReason.ForceHalt))
                .Build();

            VirtualMachine.Halt();

            hook.Verify(Called.Once());
        }
    }
}
