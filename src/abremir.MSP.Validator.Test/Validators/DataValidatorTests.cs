using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Validator.Test.Validators
{
    public class DataValidatorTests
    {
        [Fact]
        public void Validate_NoDataDeclarations_ReturnsMemoryEmptyWarning()
        {
            var result = DataValidator.Validate([], []);

            Check.That(result.Errors).IsEmpty();
            Check.That(result.Warnings.Single().Warning).Is(Warning.DataNoVariablesDeclared);
        }

        [Fact]
        public void Validate_DataAllocationOverflowsMemoryCapacity_ReturnsMemoryCapacityExhaustedError()
        {
            var data = new ParsedData(0, "x", Constants.MemoryCapacity - 1, 2, null);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.DataMemoryCapacityExhausted);
        }

        [Fact]
        public void Validate_DataAddressIsBeyondMemoryCapacity_ReturnsInvalidAddressError()
        {
            var data = new ParsedData(0, "x", Constants.MemoryCapacity, 2, null);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.DataInvalidAddress);
        }

        [Fact]
        public void Validate_DataAddressBelongsToMemoryAlreadyReserved_ReturnsInvalidAddressError()
        {
            var dataX = new ParsedData(1, "x", 0, 10, null);
            var dataY = new ParsedData(2, "y", 5, 10, null);

            var result = DataValidator.Validate([dataY, dataX], []);

            Check.That(result.Errors.Single(error => error.Error is Error.DataInvalidSize).LineNumber).Is(dataY.LineNumber);
        }

        [Fact]
        public void Validate_DataVariableIsReDeclared_ReturnsDataRedefinitionOfVariable()
        {
            var dataX1 = new ParsedData(1, "x", 0, 10, null);
            var dataX2 = new ParsedData(2, "x", 10, 10, null);
            var dataX3 = new ParsedData(3, "x", 10, 10, null);

            var result = DataValidator.Validate([dataX2, dataX3, dataX1], []);

            Check.That(result.Errors.Where(error => error.Error is Error.DataRedefinitionOfVariable)
                .Select(error => error.LineNumber)).IsEquivalentTo(new[] { dataX2.LineNumber, dataX3.LineNumber });
        }

        [Fact]
        public void Validate_DataSizeIsZero_ReturnsInvalidSizeError()
        {
            var data = new ParsedData(0, "x", 0, 0, null);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.DataInvalidSize);
        }

        [Fact]
        public void Validate_DataSizeIsLargerThanMemoryCapacity_ReturnsInvalidSizeError()
        {
            var data = new ParsedData(0, "x", 0, Constants.MemoryCapacity + 1, null);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.DataInvalidSize);
        }

        [Fact]
        public void Validate_DataAllocationOverflowsMemoryCapacity_ReturnsInvalidSizeError()
        {
            var data = new ParsedData(0, "x", Constants.MemoryCapacity - 1, 2, null);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.DataInvalidSize);
        }

        [Fact]
        public void Validate_DataBlockOverlapsWithMemoryAlreadyReserved_ReturnsInvalidSizeError()
        {
            var dataX = new ParsedData(1, "x", 0, 10, null);
            var dataY = new ParsedData(2, "y", 5, 10, null);

            var result = DataValidator.Validate([dataY, dataX], []);

            Check.That(result.Errors.Single(error => error.Error is Error.DataInvalidSize).LineNumber).Is(dataY.LineNumber);
        }

        [Fact]
        public void Validate_DataValuesInitializedButNoValueGiven_ReturnsUnexpectedInitializationValuesError()
        {
            var data = new ParsedData(0, "x", 0, 2, []);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.DataUnexpectedInitializationValues);
        }

        [Fact]
        public void Validate_DataValuesGivenExceedDataSize_ReturnsInitializedValuesExceedReservedSpaceError()
        {
            var data = new ParsedData(0, "x", 0, 1, [1, 1]);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Errors).HasElementThatMatches(error => error.Error == Error.DataInitializedValuesExceedReservedSpace);
        }

        [Fact]
        public void Validate_DataValuesAreOutsideAllowedRange_ReturnsInitializationValueOutsideAllowedRangeError()
        {
            var dataX = new ParsedData(1, "x", 0, 1, [Constants.Min8BitValue - 1]);
            var dataY = new ParsedData(2, "y", 1, 1, [Constants.Min8BitValue]);
            var dataZ = new ParsedData(3, "z", 2, 1, [Constants.Max8BitValue + 1]);
            var dataW = new ParsedData(4, "w", 3, 1, [Constants.Max8BitValue]);
            var dataA = new ParsedData(5, "a", 4, 1, [5]);

            var result = DataValidator.Validate([dataA, dataW, dataX, dataY, dataZ], []);

            Check.That(result.Errors.Where(error => error.Error is Error.DataInitializationValueOutsideAllowedRange)
                .Select(error => error.LineNumber)).IsEquivalentTo(new[] { dataX.LineNumber, dataZ.LineNumber });
        }

        [Fact]
        public void Validate_DataValuesGivenAreLessThanTheDataSize_ReturnsUninitializedValuesWarning()
        {
            var data = new ParsedData(0, "x", 0, 2, [1]);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Warnings).HasElementThatMatches(warning => warning.Warning == Warning.DataUninitializedValues);
        }

        [Fact]
        public void Validate_DataVariableNotUsedInCode_ReturnsUnusedVariableWarning()
        {
            var data = new ParsedData(0, "x", 0, 1, [1]);

            var result = DataValidator.Validate([data], []);

            Check.That(result.Warnings).HasElementThatMatches(warning => warning.Warning == Warning.DataUnusedVariable);
        }
    }
}