using System;

namespace abremir.MSP.VirtualMachine.Models
{
    public class InputRequestedEventArgs(bool isCharacter) : EventArgs
    {
        public bool IsCharacter { get; } = isCharacter;
    }
}
