namespace abremir.MSP.Shared.Models
{
    public record ParsedData(int LineNumber, string VariableName, int Address, int Size, int[]? Values = null);
}
