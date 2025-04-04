using abremir.MSP.Assembler.Assemblers;
using abremir.MSP.Assembler.Models;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler
{
    public class Assembler(
        IDataAssembler dataAssembler,
        IProgramAssembler programAssembler) : IAssembler
    {
        private readonly IDataAssembler _dataAssembler = dataAssembler;
        private readonly IProgramAssembler _programAssembler = programAssembler;

        public AssemblerResult Assemble(IReadOnlyCollection<ParsedData> parsedData, IReadOnlyCollection<ParsedInstruction> parsedInstructions)
        {
            var assembledData = _dataAssembler.Assemble(parsedData);
            var assembledProgram = _programAssembler.Assemble(parsedInstructions, assembledData.DataVariableMap);

            var dataAddressVariableMap = assembledData.DataVariableMap.Select(keyValuePair => new DataAddressVariable(keyValuePair.Value, keyValuePair.Key)).ToList();

            return new AssemblerResult(assembledData.Data, assembledProgram.Program, assembledProgram.Errors, assembledProgram.LineAddressMap, dataAddressVariableMap);
        }
    }
}