using System;

namespace abremir.MSP.VirtualMachine.Models
{
    public class OutputEmittedEventArgs(int value, bool isCharacter) : EventArgs
    {
        public int Value { get; } = value;
        public bool IsCharacter { get; } = isCharacter;
    }
}
