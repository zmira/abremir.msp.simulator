using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine.Memory;

namespace abremir.MSP.VirtualMachine.Test.Memory
{
    public class StackTests
    {
        private readonly IStack _stack;

        public StackTests()
        {
            _stack = new Stack();
        }

        [Fact]
        public void Clear_ResetsSP()
        {
            _stack.TryPush(1);
            _stack.TryPush(2);

            var sp = _stack.SP;

            _stack.Clear();

            _stack.SP.Should().NotBe(sp);
            _stack.SP.Should().Be(Constants.MemoryCapacity - 1);
        }

        [Fact]
        public void Clear_ResetsStackData()
        {
            _stack.TryPush(1);
            _stack.TryPush(2);

            var data = _stack.StackData;

            _stack.Clear();

            _stack.StackData.Count.Should().NotBe(data.Count);
            _stack.StackData.Should().BeEmpty();
        }

        [Fact]
        public void TryPush_StackNotFull_PushesValue()
        {
            const byte value = 10;

            var result = _stack.TryPush(value);

            result.Should().BeTrue();
            _stack.SP.Should().Be(Constants.MemoryCapacity - 2);
        }

        [Fact]
        public void TryPush_StackNotFull_DecrementsSP()
        {
            var sp = _stack.SP;

            _stack.TryPush(10);

            _stack.SP.Should().BeLessThan(sp);
        }

        [Fact]
        public void TryPush_StackFull_ReturnsFalse()
        {
            for (var i = 0; i < Constants.MemoryCapacity; i++)
            {
                _stack.TryPush(0);
            }

            var result = _stack.TryPush(99);

            result.Should().BeFalse();
        }

        [Fact]
        public void TryPop_StackWithContent_ReturnsValue()
        {
            const byte value = 10;

            _stack.TryPush(value);

            var result = _stack.TryPop(out var returnValue);

            result.Should().BeTrue();
            returnValue.Should().Be(value);
            _stack.SP.Should().Be(Constants.MemoryCapacity - 1);
        }

        [Fact]
        public void TryPop_StackWithContent_IncrementsSP()
        {
            _stack.TryPush(10);

            var sp = _stack.SP;

            _stack.TryPop(out _);

            _stack.SP.Should().BeGreaterThan(sp);
        }

        [Fact]
        public void TryPop_EmptyStack_ReturnsFalse()
        {
            var result = _stack.TryPop(out _);

            result.Should().BeFalse();
        }

        [Fact]
        public void StackData_EmptyStack_ReturnsEmpty()
        {
            var result = _stack.StackData;

            result.Should().BeEmpty();
        }

        [Fact]
        public void StackData_StackWithContent_ReturnsExpected()
        {
            _stack.TryPush(1);
            _stack.TryPush(2);
            _stack.TryPush(3);
            _stack.TryPush(4);
            _stack.TryPop(out _);

            var result = _stack.StackData;

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(new[] { 3, 2, 1 });
        }
    }
}
