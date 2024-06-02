using abremir.MSP.Shared.Models;
using abremir.MSP.Validator.Models;
using abremir.MSP.Validator.Validators;

namespace abremir.MSP.Validator
{
    public class Validator : IValidator
    {
        public ValidatorResult Validate(IReadOnlyCollection<ParsedData> parsedData, IReadOnlyCollection<ParsedInstruction> parsedInstructions)
        {
            var dataValidation = DataValidator.Validate(parsedData, parsedInstructions);
            var codeValidation = CodeValidator.Validate(parsedData, parsedInstructions);

            return new ValidatorResult(
                [.. dataValidation.Errors, .. codeValidation.Errors],
                [.. dataValidation.Warnings, .. codeValidation.Warnings]);
        }
    }
}