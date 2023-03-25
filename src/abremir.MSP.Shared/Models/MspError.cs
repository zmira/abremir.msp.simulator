using abremir.MSP.Shared.Enums;

namespace abremir.MSP.Shared.Models
{
    public record MspError(Error Error, int LineNumber = 0, int? ColumnNumber = null, string? ErrorMessage = null);
}
