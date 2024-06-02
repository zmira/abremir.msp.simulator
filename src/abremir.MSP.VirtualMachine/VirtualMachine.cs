using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Models;

namespace abremir.MSP.VirtualMachine
{
    public class VirtualMachine(IVirtualMachineMemory? dataMemory, IVirtualMachineMemory? programMemory, IStack? stack) : IVirtualMachine
    {
        private readonly IStack _stack = stack ?? throw new ArgumentNullException(nameof(stack));
        private readonly IVirtualMachineMemory _dataMemory = dataMemory ?? throw new ArgumentNullException(nameof(dataMemory));
        private readonly IVirtualMachineMemory _programMemory = programMemory ?? throw new ArgumentNullException(nameof(programMemory));

        public Status Status { get; private set; } = Status.None;
        public Mode Mode { get; private set; } = Mode.None;
        public IReadOnlyCollection<byte> Data => _dataMemory.MemoryData;
        public IReadOnlyCollection<byte> Program => _programMemory.MemoryData;
        public IReadOnlyCollection<byte> Stack => _stack.StackData;
        public ushort SP => _stack.SP;
        public ushort PC { get; private set; } = 0;
        public TimeSpan InstructionExecutionDuration { get; private set; } = TimeSpan.Zero;
        public InterruptReason? InterruptedBy { get; private set; }
        public HaltReason? HaltedBy { get; private set; }

        public event EventHandler? VirtualMachineMemorySet;
        public event EventHandler? VirtualMachineReset;
        public event EventHandler<ushort>? StackPointerUpdated;
        public event EventHandler<ushort>? ProgramCounterUpdated;
        public event EventHandler<VirtualMachineHaltedEventArgs>? VirtualMachineHalted;
        public event EventHandler<InstructionExecutingEventArgs>? InstructionExecuting;
        public event EventHandler<InstructionArgumentsEventArgs>? InstructionArguments;
        public event EventHandler<InstructionExecutedEventArgs>? InstructionExecuted;
        public event EventHandler<InputRequestedEventArgs>? InputRequested;
        public event EventHandler<OutputEmittedEventArgs>? OutputEmitted;
        public event EventHandler<StatusChangedEventArgs>? StatusChanged;
        public event EventHandler<ModeChangedEventArgs>? ModeChanged;
        public event EventHandler<DataMemoryUpdatedEventArgs>? DataMemoryUpdated;

        public void SetMemory(IReadOnlyCollection<byte>? data, IReadOnlyCollection<byte>? program)
        {
            data = data ?? throw new ArgumentNullException(nameof(data));
            program = program ?? throw new ArgumentNullException(nameof(program));

            if (Status is not Status.None)
            {
                return;
            }

            _dataMemory.SetMemory([.. data]);
            _programMemory.SetMemory([.. program]);

            OnVirtualMachineMemorySet();
        }

        public void Reset(bool clearAllMemory = false)
        {
            SetMode(Mode.None);
            SetStatus(Status.None);

            HaltedBy = null;
            InterruptedBy = null;

            ClearStack();
            SetProgramCounter(0);

            OnVirtualMachineReset();

            if (clearAllMemory)
            {
                _dataMemory.Clear();
                _programMemory.Clear();

                OnVirtualMachineMemorySet();
            }
        }

        public void Run()
        {
            if (Status is Status.Running && Mode is Mode.Run)
            {
                return;
            }

            InternalRun();
        }

        public void Step()
        {
            if (RaiseInputRequestedEvent())
            {
                return;
            }

            ResetIfHalted();

            SetMode(Mode.Step);
            SetStatus(Status.Running);

            ExecuteNextInstruction();
        }

        public void Halt()
        {
            Halt(null, null, HaltReason.ForceHalt);
        }

        public void Input(sbyte value)
        {
            if (Status is not Status.Interrupted
                || (InterruptedBy is not null && InterruptedBy != InterruptReason.InputValue))
            {
                return;
            }

            if (!TryPushToStack(Operation.InputValue, ((int)value).ToTwosComplement()))
            {
                return;
            }

            Continue(Operation.InputValue);
        }

        public void InputCharacter(byte value)
        {
            if (Status is not Status.Interrupted
                || (InterruptedBy is not null && InterruptedBy != InterruptReason.InputCharacter))
            {
                return;
            }

            if (!TryPushToStack(Operation.InputCharacter, value))
            {
                return;
            }

            Continue(Operation.InputCharacter);
        }

        public void SetInstructionExecutionDuration(TimeSpan duration)
        {
            InstructionExecutionDuration = duration;
        }

        public void Suspend()
        {
            if (Status is not Status.Running || Mode is not Mode.Run)
            {
                return;
            }

            SetStatus(Status.Suspended);
        }

        protected virtual void OnVirtualMachineMemorySet()
        {
            VirtualMachineMemorySet?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnVirtualMachineReset()
        {
            VirtualMachineReset?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnVirtualMachineHalted(VirtualMachineHaltedEventArgs e)
        {
            VirtualMachineHalted?.Invoke(this, e);
        }

        protected virtual void OnInstructionExecuting(InstructionExecutingEventArgs e)
        {
            InstructionExecuting?.Invoke(this, e);
        }

        protected virtual void OnInstructionArguments(InstructionArgumentsEventArgs e)
        {
            InstructionArguments?.Invoke(this, e);
        }

        protected virtual void OnInstructionExecuted(InstructionExecutedEventArgs e)
        {
            InstructionExecuted?.Invoke(this, e);
        }

        public virtual void OnInputRequested(InputRequestedEventArgs e)
        {
            InputRequested?.Invoke(this, e);
        }

        protected virtual void OnOutputEmitted(OutputEmittedEventArgs e)
        {
            OutputEmitted?.Invoke(this, e);
        }

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        protected virtual void OnModeChanged(ModeChangedEventArgs e)
        {
            ModeChanged?.Invoke(this, e);
        }

        protected virtual void OnStackPointerUpdated(ushort stackPointer)
        {
            StackPointerUpdated?.Invoke(this, stackPointer);
        }

        protected virtual void OnProgramCounterUpdated(ushort programCounter)
        {
            ProgramCounterUpdated?.Invoke(this, programCounter);
        }

        protected virtual void OnDataMemoryUpdated(DataMemoryUpdatedEventArgs e)
        {
            DataMemoryUpdated?.Invoke(this, e);
        }

        internal async void InternalRun()
        {
            if (RaiseInputRequestedEvent())
            {
                return;
            }

            ResetIfHalted();

            SetMode(Mode.Run);
            SetStatus(Status.Running);

            while (Status is not Status.Halted
                && Status is not Status.Suspended
                && Mode is Mode.Run)
            {
                ExecuteNextInstruction();

                if (Status is Status.Halted
                    || Status is Status.Interrupted
                    || Status is Status.Suspended
                    || Mode is not Mode.Run)
                {
                    break;
                }

                if (InstructionExecutionDuration != TimeSpan.Zero)
                {
                    await Task.Delay(InstructionExecutionDuration);
                }
            }
        }

        internal void ExecuteNextInstruction()
        {
            var executeInstructionCompleted = ExecuteInstruction(PC);

            if (executeInstructionCompleted is not null)
            {
                OnInstructionExecuted(new(PC, SP, executeInstructionCompleted.Operation));

                if (executeInstructionCompleted.Operation is not Operation.Halt)
                {
                    if (executeInstructionCompleted.Operation == Operation.InputCharacter || executeInstructionCompleted.Operation == Operation.InputValue)
                    {
                        switch (executeInstructionCompleted.Operation)
                        {
                            case Operation.InputValue:
                                OnInputRequested(new(false));
                                break;
                            case Operation.InputCharacter:
                                OnInputRequested(new(true));
                                break;
                        }

                        return;
                    }

                    var newPC = executeInstructionCompleted.Address
                        ?? (ushort)(PC + executeInstructionCompleted.Operation.GetNumberOfMemoryCellsOccupied());

                    SetProgramCounter(newPC);
                }
            }
        }

        internal ExecuteInstructionCompleted? ExecuteInstruction(ushort pc)
        {
            if (Status is Status.Halted
                || Status is Status.Interrupted
                || Status is Status.Suspended)
            {
                return null;
            }

            if (!TryGetProgramValue(null, pc, out var opCode))
            {
                return null;
            }

            if (!Enum.IsDefined(typeof(Operation), (int)opCode))
            {
                Halt(opCode, null, HaltReason.UnknownOperation);

                return null;
            }

            var operation = (Operation)opCode;

            OnInstructionExecuting(new(pc, SP, operation));

            switch (operation)
            {
                case Operation.NoOperation:
                    break;
                case Operation.PushValue:
                    {
                        if (!TryGetProgramValue(operation, (ushort)(pc + 1), out var value))
                        {
                            return null;
                        }

                        OnInstructionArguments(new(pc, SP, operation, value));

                        if (!TryPushToStack(operation, value))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.PushAddress:
                    {
                        if (!TryGetProgramValue(operation, (ushort)(pc + 1), out var lsb)
                            || !TryGetProgramValue(operation, (ushort)(pc + 2), out var msb))
                        {
                            return null;
                        }

                        OnInstructionArguments(new(pc, SP, operation, new[] { lsb, msb }.ToMemoryAddress(), lsb, msb));

                        if (!TryPushToStack(operation, lsb)
                            || !TryPushToStack(operation, msb))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.LoadValue:
                    {
                        if (!TryPopFromStack(operation, out var msb)
                            || !TryPopFromStack(operation, out var lsb))
                        {
                            return null;
                        }

                        var address = new[] { lsb, msb }.ToMemoryAddress();

                        if (!TryGetDataValue(operation, (ushort)address, out var value))
                        {
                            return null;
                        }

                        if (!TryPushToStack(operation, value))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.LoadAddress:
                    {
                        if (!TryPopFromStack(operation, out var msb)
                            || !TryPopFromStack(operation, out var lsb))
                        {
                            return null;
                        }

                        var address = new[] { lsb, msb }.ToMemoryAddress();

                        if (!TryGetDataValue(operation, (ushort)address, out var newLsb)
                            || !TryGetDataValue(operation, (ushort)(address + 1), out var newMsb))
                        {
                            return null;
                        }

                        if (!TryPushToStack(operation, newLsb)
                            || !TryPushToStack(operation, newMsb))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.StoreValue:
                    {
                        if (!TryPopFromStack(operation, out var value)
                            || !TryPopFromStack(operation, out var msb)
                            || !TryPopFromStack(operation, out var lsb))
                        {
                            return null;
                        }

                        var address = new[] { lsb, msb }.ToMemoryAddress();

                        if (!TrySetDataValue(operation, (ushort)address, value))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.StoreAddress:
                    {
                        if (!TryPopFromStack(operation, out var msbValue)
                            || !TryPopFromStack(operation, out var lsbValue)
                            || !TryPopFromStack(operation, out var msb)
                            || !TryPopFromStack(operation, out var lsb))
                        {
                            return null;
                        }

                        var address = new[] { lsb, msb }.ToMemoryAddress();

                        if (!TrySetDataValue(operation, (ushort)address, lsbValue)
                            || !TrySetDataValue(operation, (ushort)(address + 1), msbValue))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.InputValue:
                    Interrupt(InterruptReason.InputValue);
                    break;
                case Operation.OutputValue:
                    {
                        if (!TryPopFromStack(operation, out var value))
                        {
                            return null;
                        }

                        OnOutputEmitted(new(value.FromTwosComplement(), false));
                    }
                    break;
                case Operation.Add:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand.FromTwosComplement() + secondOperand.FromTwosComplement();

                        if (!IsValidResult(operation, result, true))
                        {
                            return null;
                        }

                        if (!TryPushToStack(operation, result.ToTwosComplement()))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.Subtract:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand.FromTwosComplement() - secondOperand.FromTwosComplement();

                        if (!IsValidResult(operation, result, true))
                        {
                            return null;
                        }

                        if (!TryPushToStack(operation, result.ToTwosComplement()))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.Multiply:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand.FromTwosComplement() * secondOperand.FromTwosComplement();

                        if (!IsValidResult(operation, result, true))
                        {
                            return null;
                        }

                        if (!TryPushToStack(operation, result.ToTwosComplement()))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.Divide:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        if (secondOperand is 0)
                        {
                            Halt(opCode, operation, HaltReason.DivideByZero);

                            return null;
                        }

                        var result = firstOperand.FromTwosComplement() / secondOperand.FromTwosComplement();

                        if (!IsValidResult(operation, result, true))
                        {
                            return null;
                        }

                        if (!TryPushToStack(operation, result.ToTwosComplement()))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.AddAddress:
                    {
                        if (!TryPopFromStack(operation, out var offset)
                            || !TryPopFromStack(operation, out var msb)
                            || !TryPopFromStack(operation, out var lsb))
                        {
                            return null;
                        }

                        var address = new[] { lsb, msb }.ToMemoryAddress() + offset.FromTwosComplement();

                        if (!IsValidAddress(operation, address))
                        {
                            return null;
                        }

                        var addressComponents = address.ToLeastAndMostSignificantBytes();

                        if (!TryPushToStack(operation, addressComponents[0]) // LSB
                            || !TryPushToStack(operation, addressComponents[1])) // MSB
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.LogicAnd:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand is 1 && secondOperand is 1;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.LogicOr:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand is 1 || secondOperand is 1;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.LogicNot:
                    {
                        if (!TryPopFromStack(operation, out var operand))
                        {
                            return null;
                        }

                        var result = operand is 0;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.Equal:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand == secondOperand;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.NotEqual:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand != secondOperand;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.LessThan:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand < secondOperand;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.LessThanOrEqual:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand <= secondOperand;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.GreaterThan:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand > secondOperand;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.GreaterThanOrEqual:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand >= secondOperand;

                        if (!TryPushToStack(operation, (byte)(result ? 1 : 0)))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.Jump:
                    {
                        if (!TryGetProgramValue(operation, (ushort)(pc + 1), out var lsb)
                            || !TryGetProgramValue(operation, (ushort)(pc + 2), out var msb))
                        {
                            return null;
                        }

                        var address = new[] { lsb, msb }.ToMemoryAddress();

                        OnInstructionArguments(new(pc, SP, operation, address, lsb, msb));

                        if (!IsValidAddress(operation, address))
                        {
                            return null;
                        }

                        return new(operation, (ushort)address);
                    }
                case Operation.JumpIfFalse:
                    {
                        if (!TryPopFromStack(operation, out var value))
                        {
                            return null;
                        }

                        if (value is 0)
                        {
                            if (!TryGetProgramValue(operation, (ushort)(pc + 1), out var lsb)
                                || !TryGetProgramValue(operation, (ushort)(pc + 2), out var msb))
                            {
                                return null;
                            }

                            var address = new[] { lsb, msb }.ToMemoryAddress();

                            OnInstructionArguments(new(pc, SP, operation, address, lsb, msb));

                            if (!IsValidAddress(operation, address))
                            {
                                return null;
                            }

                            return new(operation, (ushort)address);
                        }
                    }
                    break;
                case Operation.Call:
                    {
                        if (!TryGetProgramValue(operation, (ushort)(pc + 1), out var lsb)
                            || !TryGetProgramValue(operation, (ushort)(pc + 2), out var msb))
                        {
                            return null;
                        }

                        var address = new[] { lsb, msb }.ToMemoryAddress();

                        OnInstructionArguments(new(pc, SP, operation, address, lsb, msb));

                        var nextInstructionAddress = pc + Operation.Call.GetNumberOfMemoryCellsOccupied();

                        if (!IsValidAddress(operation, address)
                            || !IsValidAddress(operation, nextInstructionAddress))
                        {
                            return null;
                        }

                        var addressComponents = nextInstructionAddress.ToLeastAndMostSignificantBytes();

                        if (!TryPushToStack(operation, addressComponents[0]) // LSB
                            || !TryPushToStack(operation, addressComponents[1])) // MSB
                        {
                            return null;
                        }

                        return new(operation, (ushort)address);
                    }
                case Operation.ReturnFromCall:
                    {
                        if (!TryPopFromStack(operation, out var msb)
                            || !TryPopFromStack(operation, out var lsb))
                        {
                            return null;
                        }

                        var address = new[] { lsb, msb }.ToMemoryAddress();

                        if (!IsValidAddress(operation, address))
                        {
                            return null;
                        }

                        return new(operation, (ushort)address);
                    }
                case Operation.Halt:
                    Halt(opCode, operation, HaltReason.HaltInstruction);
                    break;
                case Operation.InputCharacter:
                    Interrupt(InterruptReason.InputCharacter);
                    break;
                case Operation.OutputCharacter:
                    {
                        if (!TryPopFromStack(operation, out var value))
                        {
                            return null;
                        }

                        OnOutputEmitted(new(value, true));
                    }
                    break;
                case Operation.BitwiseAnd:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand & secondOperand;

                        if (!TryPushToStack(operation, (byte)result))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.BitwiseOr:
                    {
                        if (!TryPopFromStack(operation, out var secondOperand)
                            || !TryPopFromStack(operation, out var firstOperand))
                        {
                            return null;
                        }

                        var result = firstOperand | secondOperand;

                        if (!TryPushToStack(operation, (byte)result))
                        {
                            return null;
                        }
                    }
                    break;
                case Operation.BitwiseNot:
                    {
                        if (!TryPopFromStack(operation, out var operand))
                        {
                            return null;
                        }

                        var result = ~operand;

                        if (!TryPushToStack(operation, (byte)result))
                        {
                            return null;
                        }
                    }
                    break;
            }

            return new(operation);
        }

        internal void Halt(byte? opcode, Operation? operation, HaltReason reason)
        {
            if (Status is Status.Halted)
            {
                return;
            }

            HaltedBy = reason;

            SetStatus(Status.Halted);

            OnVirtualMachineHalted(new(PC, SP, opcode, operation, reason));
        }

        internal void Interrupt(InterruptReason interruptedBy)
        {
            if (Status is Status.Interrupted
                || Status is Status.Halted
                || Status is Status.Suspended)
            {
                return;
            }

            InterruptedBy = interruptedBy;

            SetStatus(Status.Interrupted);
        }

        internal bool IsValidResult(Operation operation, int result, bool arithmetic = false)
        {
            if (result < Constants.Min8BitValue)
            {
                Halt((byte)operation, operation, HaltReason.UnderflowError);

                return false;
            }

            if ((arithmetic && (result > sbyte.MaxValue)) || result > Constants.Max8BitValue)
            {
                Halt((byte)operation, operation, HaltReason.OverflowError);

                return false;
            }

            return true;
        }

        internal bool IsValidAddress(Operation? operation, int address)
        {
            if (address < 0 || address >= Constants.MemoryCapacity)
            {
                Halt((byte?)operation, operation, HaltReason.MemoryAddressViolation);

                return false;
            }

            return true;
        }

        internal void SetStatus(Status newStatus)
        {
            if (Status == newStatus)
            {
                return;
            }

            Status = newStatus;

            OnStatusChanged(new(newStatus));
        }

        internal void SetMode(Mode newMode)
        {
            if (Mode == newMode)
            {
                return;
            }

            Mode = newMode;

            OnModeChanged(new(newMode));
        }

        internal void Continue(Operation continueFromOperation)
        {
            if (Status is not Status.Interrupted
                || (continueFromOperation is Operation.InputCharacter && InterruptedBy is not InterruptReason.InputCharacter)
                || (continueFromOperation is Operation.InputValue && InterruptedBy is not InterruptReason.InputValue))
            {
                return;
            }

            var newPC = (ushort)(PC + continueFromOperation.GetNumberOfMemoryCellsOccupied());

            if (!SetProgramCounter(newPC))
            {
                return;
            }

            InterruptedBy = null;

            SetStatus(Status.Running);

            if (Mode == Mode.Run)
            {
                InternalRun();
            }
        }

        internal bool TryPopFromStack(Operation operation, out byte value)
        {
            if (!_stack.TryPop(out value))
            {
                Halt((byte?)operation, operation, HaltReason.StackEmpty);

                return false;
            }

            OnStackPointerUpdated(SP);

            return true;
        }

        internal bool TryPushToStack(Operation operation, byte value)
        {
            if (!_stack.TryPush(value))
            {
                Halt((byte)operation, operation, HaltReason.StackFull);

                return false;
            }

            OnStackPointerUpdated(SP);

            return true;
        }

        internal bool TryGetProgramValue(Operation? operation, ushort address, out byte value)
        {
            if (!_programMemory.TryGet(address, out value))
            {
                Halt((byte?)operation, operation, HaltReason.MemoryAddressViolation);

                return false;
            }

            return true;
        }

        internal bool TryGetDataValue(Operation operation, ushort address, out byte value)
        {
            if (!_dataMemory.TryGet(address, out value))
            {
                Halt((byte?)operation, operation, HaltReason.MemoryAddressViolation);

                return false;
            }

            return true;
        }

        internal bool TrySetDataValue(Operation operation, ushort address, byte value)
        {
            if (!_dataMemory.TrySet(address, value))
            {
                Halt((byte?)operation, operation, HaltReason.MemoryAddressViolation);

                return false;
            }

            OnDataMemoryUpdated(new(address, value));

            return true;
        }

        internal void ClearStack()
        {
            _stack.Clear();

            OnStackPointerUpdated(SP);
        }

        internal bool SetProgramCounter(ushort programCounter)
        {
            if (!IsValidAddress(null, programCounter))
            {
                return false;
            }

            PC = programCounter;

            OnProgramCounterUpdated(PC);

            return true;
        }

        internal bool RaiseInputRequestedEvent()
        {
            if (Status is not Status.Interrupted || InterruptedBy is null)
            {
                return false;
            }

            switch (InterruptedBy)
            {
                case InterruptReason.InputValue:
                    OnInputRequested(new(false));
                    break;
                case InterruptReason.InputCharacter:
                    OnInputRequested(new(true));
                    break;
            }

            return true;
        }

        internal void ResetIfHalted()
        {
            if (Status is not Status.Halted)
            {
                return;
            }

            ClearStack();
            SetProgramCounter(0);

            HaltedBy = null;
        }
    }
}
