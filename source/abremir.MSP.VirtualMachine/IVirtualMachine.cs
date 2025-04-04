using System;
using System.Collections.Generic;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;

namespace abremir.MSP.VirtualMachine
{
    public interface IVirtualMachine
    {
        Status Status { get; }
        Mode Mode { get; }
        IReadOnlyCollection<byte> Data { get; }
        IReadOnlyCollection<byte> Program { get; }
        IReadOnlyCollection<byte> Stack { get; }
        ushort SP { get; }
        ushort PC { get; }
        TimeSpan InstructionExecutionDuration { get; }

        event EventHandler? VirtualMachineMemorySet;
        event EventHandler? VirtualMachineReset;
        event EventHandler<ushort>? StackPointerUpdated;
        event EventHandler<ushort>? ProgramCounterUpdated;
        event EventHandler<VirtualMachineHaltedEventArgs>? VirtualMachineHalted;
        event EventHandler<InstructionExecutingEventArgs>? InstructionExecuting;
        event EventHandler<InstructionArgumentsEventArgs>? InstructionArguments;
        event EventHandler<InstructionExecutedEventArgs>? InstructionExecuted;
        event EventHandler<InputRequestedEventArgs>? InputRequested;
        event EventHandler<OutputEmittedEventArgs>? OutputEmitted;
        event EventHandler<StatusChangedEventArgs>? StatusChanged;
        event EventHandler<ModeChangedEventArgs>? ModeChanged;
        event EventHandler<DataMemoryUpdatedEventArgs>? DataMemoryUpdated;

        void SetMemory(IReadOnlyCollection<byte> data, IReadOnlyCollection<byte> program);
        void Reset(bool clearAllMemory = false);
        void Run();
        void Step();
        void Halt();
        void Input(sbyte value);
        void InputCharacter(byte value);
        void SetInstructionExecutionDuration(TimeSpan duration);
        void Suspend();
    }
}
