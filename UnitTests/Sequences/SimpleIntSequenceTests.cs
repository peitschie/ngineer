using NGineer.Sequences;
using NUnit.Framework;

namespace NGineer.UnitTests.Sequences
{
    [TestFixture]
    public class SimpleIntSequenceTests
    {
        private SimpleIntSequence _sequence;
        private Counter _counter;

        [SetUp]
        public void SetUp()
        {
            _counter = new Counter();
            _sequence = new SimpleIntSequence(_counter);
        }

        [Test]
        public void ShouldResetOnChange_ResetsWhenTold()
        {
            var delegateCalled = false;
            var result = false;
            _sequence.ResetOnChangeTo((o, b, s) =>
                {
                    delegateCalled = true;
                    return result;
                });

            Assert.AreEqual(0, _sequence.Next(null, null, null));
            Assert.AreEqual(1, _sequence.Next(null, null, null));
            Assert.AreEqual(2, _sequence.Next(null, null, null));
            Assert.AreEqual(3, _sequence.Next(null, null, null));

            result = true;
            Assert.AreEqual(0, _sequence.Next(null, null, null));
        }
    }
}