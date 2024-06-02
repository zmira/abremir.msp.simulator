using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;

namespace abremir.MSP.VirtualMachine.Test.Helpers
{
    internal class VirtualMachineBuilder
    {
        private IStack _stack = new Stack();
        private IVirtualMachineMemory _dataMemory = new VirtualMachineMemory();
        private IVirtualMachineMemory _programMemory = new VirtualMachineMemory(true);
        private byte[] _data = [];
        private byte[] _program = [];
        private Status? _status;
        private Mode? _mode;

        public VirtualMachineBuilder(
            IVirtualMachineMemory? dataMemory = null,
            IVirtualMachineMemory? programMemory = null,
            IStack? stack = null,
            byte[]? data = null,
            byte[]? program = null)
        {
            if (dataMemory is not null)
            {
                _dataMemory = dataMemory;
            }

            if (programMemory is not null)
            {
                _programMemory = programMemory;
            }

            if (stack is not null)
            {
                _stack = stack;
            }

            if (data is not null)
            {
                _data = data;
            }

            if (program is not null)
            {
                _program = program;
            }
        }

        public VirtualMachineBuilder WithStack(IStack stack)
        {
            _stack = stack;

            return this;
        }

        public VirtualMachineBuilder WithDataMemory(IVirtualMachineMemory memory)
        {
            _dataMemory = memory;

            return this;
        }

        public VirtualMachineBuilder WithProgramMemory(IVirtualMachineMemory memory)
        {
            _programMemory = memory;

            return this;
        }

        public VirtualMachineBuilder WithData(byte[] data)
        {
            _data = data;

            return this;
        }

        public VirtualMachineBuilder WithProgram(byte[] program)
        {
            _program = program;

            return this;
        }

        public VirtualMachineBuilder WithStatus(Status status)
        {
            _status = status;

            return this;
        }

        public VirtualMachineBuilder WithMode(Mode mode)
        {
            _mode = mode;

            return this;
        }

        public VirtualMachine Build()
        {
            var virtualMachine = new VirtualMachine(_dataMemory, _programMemory, _stack);
            virtualMachine.SetMemory(_data, _program);

            if (_status is not null)
            {
                virtualMachine.SetStatus(_status.Value);
            }

            if (_mode is not null)
            {
                virtualMachine.SetMode(_mode.Value);
            }

            return virtualMachine;
        }
    }
}
