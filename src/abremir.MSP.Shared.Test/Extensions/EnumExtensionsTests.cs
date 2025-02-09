using System.ComponentModel;

namespace abremir.MSP.Shared.Test.Extensions
{
    public class EnumExtensionsTests
    {
        public const string EnumValueDescription = "This is the description for an enum value";

        [Fact]
        public void GetDescription_ForEnumWithDescription_ReturnsDescription()
        {
            var result = TestEnumExtensionEnum.EnumValueWithDescription.GetDescription();

            Check.That(EnumValueDescription).Is(result);
        }

        [Fact]
        public void GetDescription_ForEnumWithoutDescription_ReturnsStringRepresentationOfEnumValue()
        {
            var result = TestEnumExtensionEnum.EnumValueWithoutDescription.GetDescription();

            Check.That(nameof(TestEnumExtensionEnum.EnumValueWithoutDescription)).Is(result);
        }
    }

    public enum TestEnumExtensionEnum
    {
        [Description(EnumExtensionsTests.EnumValueDescription)]
        EnumValueWithDescription,
        EnumValueWithoutDescription
    }
}
