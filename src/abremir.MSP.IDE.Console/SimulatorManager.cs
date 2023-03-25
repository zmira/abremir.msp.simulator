using System;
using System.Collections.Generic;
using System.Linq;
using abremir.MSP.Assembler.Assemblers;
using abremir.MSP.Compiler;
using abremir.MSP.Compiler.Models;
using abremir.MSP.Parser.Parsers;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.Shared.Models;
using abremir.MSP.VirtualMachine;
using abremir.MSP.VirtualMachine.Memory;

namespace abremir.MSP.IDE.Console
{
    internal class SimulatorManager
    {
        public IVirtualMachine VirtualMachine { get; }
        public ICompiler Compiler { get; }
        public ICollection<DataAddressVariable> DataAddressVariableMap { get; private set; }

        private IReadOnlyCollection<byte> _originalData = new List<byte>();
        private IReadOnlyCollection<byte> _originalProgram = new List<byte>();

        public event Action<string>? OutputEmitted;

        public SimulatorManager()
        {
            VirtualMachine = InitializeVirtualMachine();
            Compiler = InitializeCompiler();
            DataAddressVariableMap = new List<DataAddressVariable>();
        }

        private static IVirtualMachine InitializeVirtualMachine()
        {
            return new VirtualMachine.VirtualMachine(
                new VirtualMachineMemory(),
                new VirtualMachineMemory(true),
                new Stack());
        }

        private static ICompiler InitializeCompiler()
        {
            return new Compiler.Compiler(
                new Parser.Parser(new Tokenizer(), new TokenListParser()),
                new Validator.Validator(),
                new Assembler.Assembler(new DataAssembler(), new ProgramAssembler()));
        }

        private void OutputCompilationResult(CompilerResult compilerResult)
        {
            if (OutputEmitted is not null)
            {
                var output = compilerResult.Warnings.Select(warning => (Type: nameof(Warning), warning.LineNumber, Outcome: warning.Warning.GetDescription(), Message: (string?)null));
                output = output.Concat(compilerResult.Errors.Select(error => (Type: nameof(Error), error.LineNumber, Outcome: error.Error.GetDescription(), Message: error.ErrorMessage)));

                output = output.OrderBy(x => x.LineNumber);

                foreach (var outcome in output)
                {
                    OutputEmitted.Invoke($"Ln {outcome.LineNumber} - {outcome.Type} - {outcome.Outcome}{(outcome.Message is null ? string.Empty : " - ")}{outcome.Message}{Environment.NewLine}");
                }
            }
        }

        public bool Compile(string? source)
        {
            if (source is not null)
            {
                source = source.TrimEnd() + Environment.NewLine;
            }

            VirtualMachine.Reset(true);

            var result = Compiler.Compile(source);

            OutputCompilationResult(result);

            if (result.Errors.Count is 0)
            {
                DataAddressVariableMap = result.DataAddressVariableMap;
                _originalData = result.Data;
                _originalProgram = result.Program;

                VirtualMachine.SetMemory(result.Data, result.Program);

                return true;
            }

            return false;
        }

        public void Reset()
        {
            VirtualMachine.Reset();
            VirtualMachine.SetMemory(_originalData, _originalProgram);
        }

        public void New()
        {
            DataAddressVariableMap = new List<DataAddressVariable>();
            VirtualMachine.Reset(true);
        }

        public void Output(string text)
        {
            OutputEmitted?.Invoke(text);
        }

        public void Run(string? source)
        {
            if (Compile(source))
            {
                VirtualMachine.Run();
            }
        }
    }
}
