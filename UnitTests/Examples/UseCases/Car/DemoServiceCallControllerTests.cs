using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NGineer.UnitTests.Examples.UseCases.Car
{
    [TestFixture]
    public class DemoServiceCallControllerTests
    {
        private IBuilder _builder; // Stores the configured builder for each test run
        private Car _car; // Created only as it is a commonly used variable

        [SetUp]
        public void SetUp()
        {
            // In order to prevent different data being generated depending on which tests are run,
            // it is good practice to create a new Builder for each particular test.  You don't need
            // to create multiple builders within a test however

            // Want the default car to be "good" in all ways so each test can break it
            // only in the way required
            _builder = new Builder()
                .SetCollectionSize<Wheel>(4,4) // Only dealing with standard cars for now
                .For<Wheel>().Set(x => x.IsFlat, false) // No flat tyres
                .For<Car>() // Car should not require an oil change for distance or date
                    .Set(x => x.DistanceSinceLastOilChange, (obj, builder, session) => session.Random.NextInRange(0, DemoServiceCallController.DistanceBetweenOilChanges))
                    .Set(x => x.LastOilChange, (obj, builder, session) => DateTime.Now.Subtract(TimeSpan.FromDays(session.Random.NextInRange(0, DemoServiceCallController.DaysSinceOilChange))))
                ;
        }

        [Test]
        public void CarWithNoIssues_DoesNotRequireService()
        {
            // Standard non-broken car as defined by default
            _car = _builder.Build<Car>();

            Assert.IsFalse(DemoServiceCallController.RequiresService(_car));
        }
        
        [Test]
        public void CarWithOneFlatTire_RequiresService()
        {
            // Only want a single tire to be flat, all 3 other tires should be ok
            var values = new Queue<bool>(new []{false, true, false, false});

            _car = _builder
                // Each time a wheel is constructed, the next available value is pop'ed and returned
                .For<Wheel>().Set(x => x.IsFlat, values.Dequeue) 
                .Build<Car>();

            Assert.IsTrue(DemoServiceCallController.RequiresService(_car));
        }

        [Test]
        public void CarHasGoneFurtherThanAllowedDistanceBetweenOilChanges_RequiresService()
        {
            _car = _builder
                .For<Car>().Set(x => x.DistanceSinceLastOilChange, DemoServiceCallController.DistanceBetweenOilChanges + 1)
                .Build<Car>();

            Assert.IsTrue(DemoServiceCallController.RequiresService(_car));
        }

        [Test]
        public void CarHasBeenMoreThanAllowedNumberOfDaysSinceOilChange_RequiresService()
        {
            _car = _builder
                .For<Car>().Set(x => x.LastOilChange, DateTime.Now.Subtract(TimeSpan.FromDays(DemoServiceCallController.DaysSinceOilChange + 1)))
                .Build<Car>();

            Assert.IsTrue(DemoServiceCallController.RequiresService(_car));
        }
    }
}