using System.ComponentModel;
using abremir.MSP.Shared.Constants;

namespace abremir.MSP.Parser.Enums
{
    internal enum SegmentType
    {
        [Description(Constants.DataSegment)]
        Data,
        [Description(Constants.CodeSegment)]
        Code
    }
}
