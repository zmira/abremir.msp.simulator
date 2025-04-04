using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine.Memory;

namespace abremir.MSP.VirtualMachine.Test.Memory
{
    [TestClass]
    public class StackTests
    {
        private readonly Stack _stack;

        public StackTests()
        {
            _stack = new Stack();
        }

        [TestMethod]
        public void Clear_ResetsSP()
        {
            _stack.TryPush(1);
            _stack.TryPush(2);

            var sp = _stack.SP;

            _stack.Clear();

            Check.That(_stack.SP).IsNotEqualTo(sp);
            Check.That(_stack.SP).Is((ushort)(Constants.MemoryCapacity - 1));
        }

        [TestMethod]
        public void Clear_ResetsStackData()
        {
            _stack.TryPush(1);
            _stack.TryPush(2);

            var data = _stack.StackData;

            _stack.Clear();

            Check.That(_stack.StackData).Not.CountIs(data.Count);
            Check.That(_stack.StackData).IsEmpty();
        }

        [TestMethod]
        public void TryPush_StackNotFull_PushesValue()
        {
            const byte value = 10;

            var result = _stack.TryPush(value);

            Check.That(result).IsTrue();
            Check.That(_stack.SP).Is((ushort)(Constants.MemoryCapacity - 2));
        }

        [TestMethod]
        public void TryPush_StackNotFull_DecrementsSP()
        {
            var sp = _stack.SP;

            _stack.TryPush(10);

            Check.That(_stack.SP).IsStrictlyLessThan(sp);
        }

        [TestMethod]
        public void TryPush_StackFull_ReturnsFalse()
        {
            for (var i = 0; i < Constants.MemoryCapacity; i++)
            {
                _stack.TryPush(0);
            }

            var result = _stack.TryPush(99);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void TryPop_StackWithContent_ReturnsValue()
        {
            const byte value = 10;

            _stack.TryPush(value);

            var result = _stack.TryPop(out var returnValue);

            Check.That(result).IsTrue();
            Check.That(returnValue).Is(value);
            Check.That(_stack.SP).Is((ushort)(Constants.MemoryCapacity - 1));
        }

        [TestMethod]
        public void TryPop_StackWithContent_IncrementsSP()
        {
            _stack.TryPush(10);

            var sp = _stack.SP;

            _stack.TryPop(out _);

            Check.That(_stack.SP).IsStrictlyGreaterThan(sp);
        }

        [TestMethod]
        public void TryPop_EmptyStack_ReturnsFalse()
        {
            var result = _stack.TryPop(out _);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void StackData_EmptyStack_ReturnsEmpty()
        {
            var result = _stack.StackData;

            Check.That(result).IsEmpty();
        }

        [TestMethod]
        public void StackData_StackWithContent_ReturnsExpected()
        {
            _stack.TryPush(1);
            _stack.TryPush(2);
            _stack.TryPush(3);
            _stack.TryPush(4);
            _stack.TryPop(out _);

            var result = _stack.StackData;

            Check.That(result).Not.IsEmpty();
            Check.That(result).Is(new byte[] { 3, 2, 1 });
        }
    }
}
