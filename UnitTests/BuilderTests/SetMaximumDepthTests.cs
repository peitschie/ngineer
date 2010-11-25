using System;
using System.Collections.Generic;
using System.Text;
using NGineer.Internal;
using NGineer.Utils;
using NUnit.Framework;
using NGineer.Exceptions;
using NGineer.UnitTests.Types;

namespace NGineer.UnitTests.BuilderTests
{
    [TestFixture]
    public class SetMaximumDepthTests
    {
        [Test]
        public void Build_MaximumRecursionLevel_Settable_0CreatesObj()
        {
            var newClass = new Builder(1).SetMaximumDepth(0).Build<RecursiveClass>();

            Assert.IsNotNull(newClass);
            Assert.IsNull(newClass.RecursiveReference);
        }

        [Test]
        public void Build_MaximumRecursionLevel_Settable_1CreatesObjAndProperties()
        {
            var newClass = new Builder(1).SetMaximumDepth(1).Build<RecursiveClass>();

            Assert.IsNotNull(newClass);
            Assert.IsNotNull(newClass.RecursiveReference);
            Assert.IsNull(newClass.RecursiveReference.RecursiveReference);
        }

        [Test]
        public void Build_MaximumRecursionLevel_Settable()
        {
            var newClass = new Builder(1).SetMaximumDepth(3).Build<RecursiveClass>();

            Assert.IsNotNull(newClass.RecursiveReference);
            Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference);
            Assert.IsNotNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference);
            Assert.IsNull(newClass.RecursiveReference.RecursiveReference.RecursiveReference.RecursiveReference);
        }

        [Test]
        public void DefaultValue_ThrowsExceptionWhenReached()
        {
            var wrappedException = Assert.Throws<WrappedBuilderException>(() => new Builder(1).Build<RecursiveClass>());
            Assert.IsInstanceOf<DepthExceededException>(wrappedException.InnerException);
            var exception = (DepthExceededException)wrappedException.InnerException;
            Assert.IsTrue(exception.Message.Contains("Maximum build depth of 5 was exceeded"),
                          exception.Message);
        }

        [Test]
        public void SetMaximumDepth_ManuallySet_CanThrowExceptionWhenReached()
        {
            var wrappedException = Assert.Throws<WrappedBuilderException>(() => new Builder(1)
                .SetMaximumDepth(10)
                .ThrowsWhenMaximumDepthReached()
                .Build<RecursiveClass>());
            Assert.IsInstanceOf<DepthExceededException>(wrappedException.InnerException);
            var exception = (DepthExceededException)wrappedException.InnerException;
            Assert.IsTrue(exception.Message.Contains("Maximum build depth of 10 was exceeded"),
                          exception.Message);
        }

        [Test]
        public void Build_MaximumRecursionLevel_ChildContainer_SetValueBuilderEnforced()
        {
        	var sessions = new List<BuildSession>();
        	var newClass = new Builder(1)
                .SetMaximumDepth(2)
				.For<TestClassFourDeep>().Do((t, b, s) =>
                {
                    sessions.Add(s);
                    t.PropertyTestClass = b.CreateNew()
                        .For<TestClassThreeDeep>().Do((t1, b1, s1) =>
                        {
                            sessions.Add(s1);
                            t1.PropertyTestClass = b1.CreateNew()
                                .For<TestClass>().Do((t2, b2, s2) =>
                                {
                                    sessions.Add(s2);
                                    t2.Property2 = b2.CreateNew().Build<TestClass2>();
                                })
                                .Build<TestClass>();
                        })
                        .Build<TestClassThreeDeep>();
                })
                .Build<TestClassFourDeep>();

            Assert.IsNotNull(newClass.PropertyTestClass);
            Assert.IsNotNull(newClass.PropertyTestClass.PropertyTestClass);
            Assert.IsNull(newClass.PropertyTestClass.PropertyTestClass.Property2);
        }

        [Test]
        public void Build_MaximumRecursionLevel_SetValueBuilderEnforced()
        {
            List<int> buildDepth = new List<int>();
            var newClass = new Builder(1)
                .SetMaximumDepth(2)
                .For<TestClassFourDeep>().Do((type, b, s) =>
                {
                    buildDepth.Add(s.BuildDepth);
                    type.PropertyTestClass = b
                        .CreateNew()
                        .For<TestClassThreeDeep>().Do((type1, b1, s1) =>
                        {
                            buildDepth.Add(s1.BuildDepth);
                            type1.PropertyTestClass = b1
                                .CreateNew()
                                .For<TestClass>().Do((type2, b2, s2) =>
                                {
                                    buildDepth.Add(s2.BuildDepth);
                                    type2.Property2 = b2.Build<TestClass2>();
                                })
                                .Build<TestClass>();
                        })
                        .Build<TestClassThreeDeep>();
                })
                .Build<TestClassFourDeep>();

            Assert.AreEqual(new int[] { 0, 1, 2 }, buildDepth.ToArray());
            Assert.IsNotNull(newClass.PropertyTestClass);
            Assert.IsNotNull(newClass.PropertyTestClass.PropertyTestClass);
            Assert.IsNull(newClass.PropertyTestClass.PropertyTestClass.Property2);
        }

        [Test]
        public void Hierarchy_BuildDepthInheritedFromParent()
        {
            var builder = new Builder(1);
            var builder2 = builder.CreateNew();
            builder.SetMaximumDepth(3);
            Assert.AreEqual(builder.BuildDepth, builder2.BuildDepth);
        }

        [Test]
        public void Hierarchy_ChildCanBeOverridden()
        {
            var builder = new Builder(1);
            var builder2 = builder.CreateNew();
            builder.SetMaximumDepth(3);
            builder2.SetMaximumDepth(5);

            Assert.AreNotEqual(builder.BuildDepth, builder2.BuildDepth);
            Assert.AreEqual(3, builder.BuildDepth);
            Assert.AreEqual(5, builder2.BuildDepth);
        }

        [Test]
        public void List_DepthSetTo0_CreatesEmptyArray()
        {
            var list = new Builder()
                .SetMaximumDepth(0)
                .Build<string[]>();

            Assert.AreEqual(0, list.Length);
            foreach(var entry in list)
            {
                Assert.IsNotNull(entry);
            }
        }

        [Test]
        public void List_DepthSetTo1_CreatesEmptyArray()
        {
            var list = new Builder()
                .SetMaximumDepth(1)
                .Build<string[]>();

            Assert.AreEqual(0, list.Length);
            foreach(var entry in list)
            {
                Assert.IsNotNull(entry);
            }
        }

        [Test]
        public void List_DepthSetTo2_PopulatesListAsNormal()
        {
            var list = new Builder()
                .SetMaximumDepth(2)
                .Build<string[]>();

            Assert.AreNotEqual(0, list.Length);
            foreach(var entry in list)
            {
                Assert.IsNotNull(entry);
            }
        }
    }
}