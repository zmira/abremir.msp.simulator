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
            Assert.Throws<ArgumentNullException>(() => VirtualMachine.SetMemory(null, Array.Empty<byte>()));
        }

        [Fact]
        public void SetMemory_NullProgram_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => VirtualMachine.SetMemory(Array.Empty<byte>(), null));
        }

        [Fact]
        public void SetMemory_StatusIsNotNone_DoesNothing()
        {
            var data = new byte[] { 222 };
            var program = new byte[] { (byte)Operation.NoOperation };

            VirtualMachine.SetMemory(data, program);
            VirtualMachine.Step();

#pragma warning disable IDE0230 // Use UTF-8 string literal
            VirtualMachine.SetMemory(new byte[] { 111 }, new byte[] { 123 });
#pragma warning restore IDE0230 // Use UTF-8 string literal

            VirtualMachine.Data.Take(data.Length).Should().BeEquivalentTo(data);
            VirtualMachine.Program.Take(program.Length).Should().BeEquivalentTo(program);
        }

        [Fact]
        public void SetMemory_StatusIsNone_SetsDataAndProgramMemory()
        {
            var data = new byte[] { 222, 111, 3 };
            var program = new byte[] { (byte)Operation.PushValue, 1, (byte)Operation.PushValue, 1, (byte)Operation.Add };

            VirtualMachine.SetMemory(data, program);

            VirtualMachine.Data.Take(data.Length).Should().BeEquivalentTo(data);
            VirtualMachine.Program.Take(program.Length).Should().BeEquivalentTo(program);
        }

        [Fact]
        public void SetMemory_StatusIsNone_RaisesVirtualMachineMemorySetEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook((virtualMachine, handler) => virtualMachine.VirtualMachineMemorySet += handler)
                .Build();

            VirtualMachine.SetMemory(Array.Empty<byte>(), Array.Empty<byte>());

            hook.Verify(Called.Once());
        }
    }
}
