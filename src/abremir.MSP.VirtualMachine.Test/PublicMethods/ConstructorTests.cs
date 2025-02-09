using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class ConstructorTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Ctor_NullDataMemory_ThrowsArgumentNullException()
        {
            Check.ThatCode(() => new VirtualMachine(null, Substitute.For<IVirtualMachineMemory>(), Substitute.For<IStack>())).Throws<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullProgramMemory_ThrowsArgumentNullException()
        {
            Check.ThatCode(() => new VirtualMachine(Substitute.For<IVirtualMachineMemory>(), null, Substitute.For<IStack>())).Throws<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullStack_ThrowsArgumentNullException()
        {
            Check.ThatCode(() => new VirtualMachine(Substitute.For<IVirtualMachineMemory>(), Substitute.For<IVirtualMachineMemory>(), null)).Throws<ArgumentNullException>();
        }
    }
}
