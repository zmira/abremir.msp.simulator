using abremir.MSP.Shared.Enums;

namespace abremir.MSP.Shared.Models
{
    public record MspWarning(Warning Warning, int LineNumber = 0, int? ColumnNumber = null);
}
