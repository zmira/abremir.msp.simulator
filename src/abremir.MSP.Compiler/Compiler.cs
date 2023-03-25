using abremir.MSP.Assembler;
using abremir.MSP.Compiler.Models;
using abremir.MSP.Parser;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Validator;

namespace abremir.MSP.Compiler
{
    public class Compiler : ICompiler
    {
        private readonly IParser _parser;
        private readonly IValidator _validator;
        private readonly IAssembler _assembler;

        public Compiler(
            IParser parser,
            IValidator validator,
            IAssembler assembler)
        {
            _parser = parser;
            _validator = validator;
            _assembler = assembler;
        }

        public CompilerResult Compile(string? source)
        {
            var compilerResult = new CompilerResult();

            if ((source?.Length ?? 0) is 0)
            {
                compilerResult.AddError(new(Error.NoSourceDetectedToAssemble));

                return compilerResult;
            }

            var parserResult = _parser.Parse(source!);

            if (parserResult.Errors.Count is not 0)
            {
                compilerResult.AddErrors(parserResult.Errors);

                return compilerResult;
            }

            compilerResult.AddWarnings(parserResult.Warnings);

            var validatorResult = _validator.Validate(parserResult.Data, parserResult.Instructions);

            if (validatorResult.Errors.Count is not 0)
            {
                compilerResult.AddErrors(validatorResult.Errors);

                return compilerResult;
            }

            compilerResult.AddWarnings(validatorResult.Warnings);

            var assemblerResult = _assembler.Assemble(parserResult.Data, parserResult.Instructions);

            if (assemblerResult.Errors.Count is not 0)
            {
                compilerResult.AddErrors(assemblerResult.Errors);

                return compilerResult;
            }

            compilerResult.SetMemory(assemblerResult.Data, assemblerResult.Program, assemblerResult.LineAddressMap, assemblerResult.DataAddressVariableMap);

            return compilerResult;
        }
    }
}