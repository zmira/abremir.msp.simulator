using abremir.MSP.Shared.Models;
using abremir.MSP.Validator.Models;

namespace abremir.MSP.Validator
{
    public interface IValidator
    {
        ValidatorResult Validate(IReadOnlyCollection<ParsedData> parsedData, IReadOnlyCollection<ParsedInstruction> parsedInstructions);
    }
}
