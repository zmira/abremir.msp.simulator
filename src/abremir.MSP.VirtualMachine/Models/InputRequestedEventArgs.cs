using System;

namespace abremir.MSP.VirtualMachine.Models
{
    public class InputRequestedEventArgs : EventArgs
    {
        public bool IsCharacter { get; }

        public InputRequestedEventArgs(bool isCharacter)
        {
            IsCharacter = isCharacter;
        }
    }
}
