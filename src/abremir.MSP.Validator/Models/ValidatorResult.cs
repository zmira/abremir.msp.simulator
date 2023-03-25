using abremir.MSP.Shared.Models;

namespace abremir.MSP.Validator.Models
{
    public class ValidatorResult
    {
        public ICollection<MspError> Errors { get; }
        public ICollection<MspWarning> Warnings { get; }

        public ValidatorResult()
        {
            Errors = new List<MspError>();
            Warnings = new List<MspWarning>();
        }

        public ValidatorResult(ICollection<MspError> errors, ICollection<MspWarning> warnings)
        {
            Errors = errors;
            Warnings = warnings;
        }

        public void AddError(MspError error)
        {
            Errors.Add(error);
        }

        public void AddWarning(MspWarning warning)
        {
            Warnings.Add(warning);
        }
    }
}
