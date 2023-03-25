using System;
using abremir.MSP.VirtualMachine.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class ModeChangedEventArgs : EventArgs
    {
        public Mode NewMode { get; }

        public ModeChangedEventArgs(Mode newMode)
        {
            NewMode = newMode;
        }
    }
}
