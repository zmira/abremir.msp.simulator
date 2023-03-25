using System;

namespace abremir.MSP.VirtualMachine.Models
{
    public class OutputEmittedEventArgs : EventArgs
    {
        public int Value { get; }
        public bool IsCharacter { get; }

        public OutputEmittedEventArgs(int value, bool isCharacter)
        {
            Value = value;
            IsCharacter = isCharacter;
        }
    }
}
