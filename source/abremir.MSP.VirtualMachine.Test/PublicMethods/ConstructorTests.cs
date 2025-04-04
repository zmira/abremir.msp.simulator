using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    [TestClass]
    public class ConstructorTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void Ctor_NullDataMemory_ThrowsArgumentNullException()
        {
            Check.ThatCode(() => new VirtualMachine(null, Substitute.For<IVirtualMachineMemory>(), Substitute.For<IStack>())).Throws<ArgumentNullException>();
        }

        [TestMethod]
        public void Ctor_NullProgramMemory_ThrowsArgumentNullException()
        {
            Check.ThatCode(() => new VirtualMachine(Substitute.For<IVirtualMachineMemory>(), null, Substitute.For<IStack>())).Throws<ArgumentNullException>();
        }

        [TestMethod]
        public void Ctor_NullStack_ThrowsArgumentNullException()
        {
            Check.ThatCode(() => new VirtualMachine(Substitute.For<IVirtualMachineMemory>(), Substitute.For<IVirtualMachineMemory>(), null)).Throws<ArgumentNullException>();
        }
    }
}
