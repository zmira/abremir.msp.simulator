using System.ComponentModel;

namespace abremir.MSP.Shared.Test.Extensions
{
    public class EnumExtensionsTests
    {
        public const string EnumValueDescription = "This is the description for an enum value";

        [Fact]
        public void GetDescription_ForEnumWithDescription_ReturnsDescription()
        {
            var result = TestEnumExtentionEnum.EnumValueWithDescription.GetDescription();

            EnumValueDescription.Should().Be(result);
        }

        [Fact]
        public void GetDescription_ForEnumWithoutDescription_ReturnsStringRepresentationOfEnumValue()
        {
            var result = TestEnumExtentionEnum.EnumValueWithoutDescription.GetDescription();

            nameof(TestEnumExtentionEnum.EnumValueWithoutDescription).Should().Be(result);
        }
    }

    public enum TestEnumExtentionEnum
    {
        [Description(EnumExtensionsTests.EnumValueDescription)]
        EnumValueWithDescription,
        EnumValueWithoutDescription
    }
}
