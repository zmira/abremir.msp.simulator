using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class SetMemoryTests : VirtualMachineTestsBase
    {
        [Fact]
        public void SetMemory_NullData_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => VirtualMachine.SetMemory(null, []));
        }

        [Fact]
        public void SetMemory_NullProgram_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => VirtualMachine.SetMemory([], null));
        }

        [Fact]
        public void SetMemory_StatusIsNotNone_DoesNothing()
        {
            var data = new byte[] { 222 };
            var program = new byte[] { (byte)Operation.NoOperation };

            VirtualMachine.SetMemory(data, program);
            VirtualMachine.Step();

            VirtualMachine.SetMemory([111], [123]);

            VirtualMachine.Data.Take(data.Length).ToArray().ShouldBeEquivalentTo(data);
            VirtualMachine.Program.Take(program.Length).ToArray().ShouldBeEquivalentTo(program);
        }

        [Fact]
        public void SetMemory_StatusIsNone_SetsDataAndProgramMemory()
        {
            var data = new byte[] { 222, 111, 3 };
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.PushValue, 1, (byte)Operation.Add };

            VirtualMachine.SetMemory(data, program);

            VirtualMachine.Data.Take(data.Length).ToArray().ShouldBeEquivalentTo(data);
            VirtualMachine.Program.Take(program.Length).ToArray().ShouldBeEquivalentTo(program);
        }

        [Fact]
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
