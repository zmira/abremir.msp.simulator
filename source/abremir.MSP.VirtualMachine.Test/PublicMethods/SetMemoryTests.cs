using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    [TestClass]
    public class SetMemoryTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void SetMemory_NullData_ThrowsArgumentNullException()
        {
            Check.ThatCode(() => VirtualMachine.SetMemory(null, [])).Throws<ArgumentNullException>();
        }

        [TestMethod]
        public void SetMemory_NullProgram_ThrowsArgumentNullException()
        {
            Check.ThatCode(() => VirtualMachine.SetMemory([], null)).Throws<ArgumentNullException>();
        }

        [TestMethod]
        public void SetMemory_StatusIsNotNone_DoesNothing()
        {
            var data = new byte[] { 222 };
            var program = new byte[] { (byte)Operation.NoOperation };

            VirtualMachine.SetMemory(data, program);
            VirtualMachine.Step();

            VirtualMachine.SetMemory([111], [123]);

            Check.That(VirtualMachine.Data.Take(data.Length)).IsEquivalentTo(data);
            Check.That(VirtualMachine.Program.Take(program.Length)).IsEquivalentTo(program);
        }

        [TestMethod]
        public void SetMemory_StatusIsNone_SetsDataAndProgramMemory()
        {
            var data = new byte[] { 222, 111, 3 };
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.PushValue, 1, (byte)Operation.Add };

            VirtualMachine.SetMemory(data, program);

            Check.That(VirtualMachine.Data.Take(data.Length)).IsEquivalentTo(data);
            Check.That(VirtualMachine.Program.Take(program.Length)).IsEquivalentTo(program);
        }

        [TestMethod]
        public void SetMemory_StatusIsNone_RaisesVirtualMachineMemorySetEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook((virtualMachine, handler) => virtualMachine.VirtualMachineMemorySet += handler)
                .Build();

            VirtualMachine.SetMemory([], []);

            hook.Verify(Called.Once());
        }
    }
}
