using NGineer.BuildHelpers;
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
            _session = new BuildSession(null, null, null, null, (Random)null);
        }

        [Test]
        public void Next_Parent_NewChildren()
        {
            PushChild(Parent, 0);
                PushChild(Child, 0);
                PopChild();
                PushChild(Child, 1);
                PopChild();
            PopChild();
        }

        [Test]
        public void Next_ParentChild_FollowedBy_NewParent_NewChildren()
        {
            PushChild(Parent, 0);
                PushChild(Child, 0);
                PopChild();
            PopChild();
            PushChild(Parent, 1);
                PushChild(Child, 0);
                PopChild();
                PushChild(Child, 1);
                PopChild();
            PopChild();
        }

        [Test]
        public void Next_ParentChildChild_FollowedBy_NewParent_NewChildren()
        {
            PushChild(Parent, 0);
                PushChild(Child, 0);
                    PushChild(Child, 0);
                    PopChild();
                    PushChild(Child, 1);
                    PopChild();
                    PushChild(Child, 2);
                    PopChild();
                PopChild();
                PushChild(Child, 1);
                PopChild();
            PopChild();
            PushChild(Parent, 1);
                PushChild(Child, 0);
                PopChild();
            PopChild();
        }

        private static object Parent
        {
            get { return new object();}
        }

        private static object Child
        {
            get { return new object(); }
        }

        private void PushChild<TType>(TType obj, int expectedDepth)
        {
            _session.PushObject(typeof(TType), obj);
            Assert.AreEqual(expectedDepth, NextInSequence());
        }

        private void PopChild()
        {
            _session.PopObject();
        }

        private int NextInSequence()
        {
            return _sequenceController.Next(null, null, _session);
        }
    }
}