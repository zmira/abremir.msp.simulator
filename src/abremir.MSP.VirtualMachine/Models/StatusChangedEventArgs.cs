using System;
using abremir.MSP.VirtualMachine.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class StatusChangedEventArgs(Status newStatus) : EventArgs
    {
        public Status NewStatus { get; } = newStatus;
    }
}
