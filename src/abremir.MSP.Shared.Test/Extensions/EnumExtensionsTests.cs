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

            EnumValueDescription.ShouldBe(result);
        }

        [Fact]
        public void GetDescription_ForEnumWithoutDescription_ReturnsStringRepresentationOfEnumValue()
        {
            var result = TestEnumExtensionEnum.EnumValueWithoutDescription.GetDescription();

            nameof(TestEnumExtensionEnum.EnumValueWithoutDescription).ShouldBe(result);
        }
    }

    public enum TestEnumExtensionEnum
    {
        [Description(EnumExtensionsTests.EnumValueDescription)]
        EnumValueWithDescription,
        EnumValueWithoutDescription
    }
}
