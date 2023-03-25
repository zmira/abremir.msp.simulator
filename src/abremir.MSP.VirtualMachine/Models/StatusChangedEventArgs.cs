using System;
using abremir.MSP.VirtualMachine.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class StatusChangedEventArgs : EventArgs
    {
        public Status NewStatus { get; }

        public StatusChangedEventArgs(Status newStatus)
        {
            NewStatus = newStatus;
        }
    }
}
