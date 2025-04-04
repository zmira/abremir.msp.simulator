using System;
using abremir.MSP.VirtualMachine.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class ModeChangedEventArgs(Mode newMode) : EventArgs
    {
        public Mode NewMode { get; } = newMode;
    }
}
