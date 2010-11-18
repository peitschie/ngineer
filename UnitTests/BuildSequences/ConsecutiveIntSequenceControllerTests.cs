using NGineer.Internal;
using NGineer.BuildSequences;
using NUnit.Framework;
using System;

namespace NGineer.UnitTests.BuildSequences
{
    [TestFixture]
    public class ConsecutiveIntSequenceControllerTests
    {
        private ConsecutiveSequenceController<object, int> _sequenceController;
        private BuildSession _session;

        [SetUp]
        public void SetUp()
        {
            _sequenceController = Sequences.SiblingConsecutiveInt();
            _session = new BuildSession(null, null, (Random)null);
        }

        [Test]
        public void Next_Parent_NewChildren()
        {
            using(PushChild(Parent, 0))
            {
                using (PushChild(Child, 0)){}
                using (PushChild(Child, 1)){}
            }
        }

        [Test]
        public void Next_ParentChild_FollowedBy_NewParent_NewChildren()
        {
            using(PushChild(Parent, 0))
            {
                using (PushChild(Child, 0)) { }
            }
            using (PushChild(Parent, 1))
            {
                using (PushChild(Child, 0)) { }
                using (PushChild(Child, 1)) { }
            }
        }

        [Test]
        public void Next_ParentChildChild_FollowedBy_NewParent_NewChildren()
        {
            using(PushChild(Parent, 0))
            {
                using(PushChild(Child, 0))
                {
                    using(PushChild(Child, 0)){}
                    using(PushChild(Child, 1)){}
                    using(PushChild(Child, 2)){}
                }
                using(PushChild(Child, 1)){}   
            }
            using (PushChild(Parent, 1))
            {
                using (PushChild(Child, 0)) { }
            }
        }

        private static object Parent
        {
            get { return new object();}
        }

        private static object Child
        {
            get { return new object(); }
        }

        private DisposableAction PushChild<TType>(TType obj, int expectedDepth)
        {
            var disposable = _session.PushObject(new ObjectBuildRecord(typeof(TType), obj, true));
            Assert.AreEqual(expectedDepth, NextInSequence());
            return disposable;
        }

        private int NextInSequence()
        {
            return _sequenceController.Next(null, null, _session);
        }
    }
}