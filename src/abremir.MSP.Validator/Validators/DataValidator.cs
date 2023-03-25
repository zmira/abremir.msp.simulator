using System.Collections;
using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Models;
using abremir.MSP.Validator.Models;

namespace abremir.MSP.Validator.Validators
{
    internal static class DataValidator
    {
        public static ValidatorResult Validate(IReadOnlyCollection<ParsedData> parsedData, IReadOnlyCollection<ParsedInstruction> parsedInstructions)
        {
            var validatorResult = new ValidatorResult();

            if (parsedData.Count is 0)
            {
                validatorResult.AddWarning(new(Warning.DataNoVariablesDeclared));

                return validatorResult;
            }

            CheckMemoryCapacityExhausted(parsedData, validatorResult);

            CheckInvalidAddress(parsedData, validatorResult);

            CheckRedefinitionOfVariabe(parsedData, validatorResult);

            CheckInvalidSize(parsedData, validatorResult);

            CheckUnexpectedInitializationValues(parsedData, validatorResult);

            CheckInitializedValuesExceedReservedSpace(parsedData, validatorResult);

            CheckInitializationValueOutsideAllowedRange(parsedData, validatorResult);

            CheckUninitializedValues(parsedData, validatorResult);

            var usedVariables = parsedInstructions
                .Where(instruction => instruction.TargetTextIdentifier is not null)
                .Select(instruction => instruction.TargetTextIdentifier)
                .ToList();

            CheckUnusedVariable(parsedData, usedVariables!, validatorResult);

            return validatorResult;
        }

        private static void CheckMemoryCapacityExhausted(IReadOnlyCollection<ParsedData> parsedData, ValidatorResult validatorResult)
        {
            var lastAllocation = parsedData.OrderByDescending(data => data.Address).First();
            var lastAllocatedAddress = lastAllocation.Address + lastAllocation.Size;

            if (lastAllocatedAddress >= Constants.MemoryCapacity)
            {
                validatorResult.AddError(
                    new(Error.DataMemoryCapacityExhausted, lastAllocation.LineNumber));
            }
        }

        private static void CheckInvalidAddress(IReadOnlyCollection<ParsedData> parsedData, ValidatorResult validatorResult)
        {
            parsedData
                .Where(data => data.Address >= Constants.MemoryCapacity)
                .ToList()
                .ForEach(data => validatorResult.AddError(
                    new(Error.DataInvalidAddress, data.LineNumber)));

            var memoryMap = new BitArray(Constants.MemoryCapacity);

            foreach (var data in parsedData.OrderBy(allocation => allocation.Address))
            {
                if (data.Address + data.Size >= Constants.MemoryCapacity)
                {
                    continue;
                }

                var tempMemoryMap = new BitArray(Constants.MemoryCapacity);

                for (var offset = 0; offset < data.Size; offset++)
                {
                    tempMemoryMap[data.Address + offset] = true;
                }

                // address is already reserved by another memory block
                if (memoryMap[data.Address])
                {
                    validatorResult.AddError(
                        new(Error.DataInvalidAddress, data.LineNumber));
                }

                memoryMap.Or(tempMemoryMap);
            }
        }

        private static void CheckRedefinitionOfVariabe(IReadOnlyCollection<ParsedData> parsedData, ValidatorResult validatorResult)
        {
            parsedData
                .GroupBy(data => data.VariableName)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.OrderBy(data => data.LineNumber).Skip(1))
                .ToList()
                .ForEach(data => validatorResult.AddError(
                    new(Error.DataRedefinitionOfVariable, data.LineNumber)));
        }

        private static void CheckInvalidSize(IReadOnlyCollection<ParsedData> parsedData, ValidatorResult validatorResult)
        {
            parsedData
                .Where(data => data.Size is 0
                    || data.Size > Constants.MemoryCapacity
                    || data.Address + data.Size >= Constants.MemoryCapacity)
                .ToList()
                .ForEach(data => validatorResult.AddError(
                    new(Error.DataInvalidSize, data.LineNumber)));

            var memoryMap = new BitArray(Constants.MemoryCapacity);

            foreach (var data in parsedData.OrderBy(allocation => allocation.Address))
            {
                if (data.Address + data.Size >= Constants.MemoryCapacity)
                {
                    continue;
                }

                var tempMemoryMap = new BitArray(Constants.MemoryCapacity);

                for (var offset = 0; offset < data.Size; offset++)
                {
                    tempMemoryMap[data.Address + offset] = true;
                }

                // memory block to reserve collides with already reserved memory
                if (((BitArray)memoryMap.Clone()).And(tempMemoryMap).Cast<bool>().Any(x => x))
                {
                    validatorResult.AddError(
                        new(Error.DataInvalidSize, data.LineNumber));
                }

                memoryMap.Or(tempMemoryMap);
            }
        }

        private static void CheckUnexpectedInitializationValues(IReadOnlyCollection<ParsedData> parsedData, ValidatorResult validatorResult)
        {
            parsedData
                .Where(data => data.Values?.Length is 0)
                .ToList()
                .ForEach(data => validatorResult.AddError(
                    new(Error.DataUnexpectedInitializationValues, data.LineNumber)));
        }

        private static void CheckInitializedValuesExceedReservedSpace(IReadOnlyCollection<ParsedData> parsedData, ValidatorResult validatorResult)
        {
            parsedData
                .Where(data => data.Values?.Length > data.Size)
                .ToList()
                .ForEach(data => validatorResult.AddError(
                    new(Error.DataInitializedValuesExceedReservedSpace, data.LineNumber)));
        }

        private static void CheckInitializationValueOutsideAllowedRange(IReadOnlyCollection<ParsedData> parsedData, ValidatorResult validatorResult)
        {
            parsedData
                .Where(data => data.Values?.Any(value => value < Constants.Min8BitValue || value > Constants.Max8BitValue) is true)
                .ToList()
                .ForEach(data => validatorResult.AddError(
                    new(Error.DataInitializationValueOutsideAllowedRange, data.LineNumber)));
        }

        private static void CheckUninitializedValues(IReadOnlyCollection<ParsedData> parsedData, ValidatorResult validatorResult)
        {
            parsedData
                .Where(data => (data.Values?.Length ?? 0) != data.Size)
                .ToList()
                .ForEach(data => validatorResult.AddWarning(
                    new(Warning.DataUninitializedValues, data.LineNumber)));
        }

        private static void CheckUnusedVariable(IReadOnlyCollection<ParsedData> parsedData, IReadOnlyCollection<string> instructionVariables, ValidatorResult validatorResult)
        {
            parsedData
                .Where(data => !instructionVariables.Contains(data.VariableName, StringComparer.InvariantCultureIgnoreCase))
                .ToList()
                .ForEach(data => validatorResult.AddWarning(
                    new(Warning.DataUnusedVariable, data.LineNumber)));
        }
    }
}
