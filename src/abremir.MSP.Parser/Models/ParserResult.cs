﻿using abremir.MSP.Shared.Models;

namespace abremir.MSP.Parser.Models
{
    public class ParserResult
    {
        public ICollection<MspError> Errors { get; } = new List<MspError>();
        public ICollection<MspWarning> Warnings { get; } = new List<MspWarning>();
        public IReadOnlyCollection<ParsedData> Data { get; private set; } = new List<ParsedData>();
        public IReadOnlyCollection<ParsedInstruction> Instructions { get; private set; } = new List<ParsedInstruction>();

        public void SetData(IReadOnlyCollection<ParsedData>? data)
        {
            if (data is null)
            {
                return;
            }

            Data = data;
        }

        public void SetInstructions(IReadOnlyCollection<ParsedInstruction>? instructions)
        {
            if (instructions is null)
            {
                return;
            }

            Instructions = instructions;
        }

        public void AddError(MspError error) => Errors.Add(error);

        public void AddWarning(MspWarning warning) => Warnings.Add(warning);
    }
}
