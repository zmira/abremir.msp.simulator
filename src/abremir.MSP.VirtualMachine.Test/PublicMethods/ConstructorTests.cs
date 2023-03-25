using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class ConstructorTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Ctor_NullDataMemory_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new VirtualMachine(null, Substitute.For<IVirtualMachineMemory>(), Substitute.For<IStack>()));
        }

        [Fact]
        public void Ctor_NullProgramMemory_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new VirtualMachine(Substitute.For<IVirtualMachineMemory>(), null, Substitute.For<IStack>()));
        }

        [Fact]
        public void Ctor_NullStack_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new VirtualMachine(Substitute.For<IVirtualMachineMemory>(), Substitute.For<IVirtualMachineMemory>(), null));
        }
    }
}
