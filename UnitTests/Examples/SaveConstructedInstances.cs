using System;
using NUnit.Framework;
namespace NGineer.UnitTests.Examples
{
    public interface ISession
    {
        void Save(DatabaseEntity obj);
    }

    public class FakeSession : ISession
    {
        public void Save(DatabaseEntity obj)
        {
            obj.IsSaved = true;
        }
    }

    public class DatabaseEntity
    {
        public bool IsSaved { get; set; }
    }

    public class ExtendedDatabaseEntity : DatabaseEntity
    {
    }

    public class ObjectPersistor : IInstanceProcessor
    {
        private readonly ISession _session;

        public ObjectPersistor(ISession session)
        {
            _session = session;
        }

        public bool IsForType(Type type)
        {
            return type == typeof(DatabaseEntity) || type.IsSubclassOf(typeof(DatabaseEntity));
        }

        public void Process(object obj, IBuilder builder, BuildSession session)
        {
            var databaseEntity = (DatabaseEntity)obj;
            // Need to guard against the entity having already been saved
            // The instance Processor is called after ever object is constructed and set,
            // but in instances where an object is being re-used, the instance processor will
            // still be called
            if(!databaseEntity.IsSaved)
            {
                _session.Save(databaseEntity);
            }
        }
    }

    [TestFixture]
    public class SaveConstructedInstances
    {
        [Test]
        public void Do ()
        {
            // Register a new instance processor to persist database entities
            var builder = new Builder()
                // AfterPopulationOf processors are run after an object is constructed and
                // all setters have been run
                .AfterPopulationOf(new ObjectPersistor(new FakeSession()));
            var customClass = builder.Build<DatabaseEntity>();
            Assert.IsTrue(customClass.IsSaved);

            var extendedClass = builder.Build<ExtendedDatabaseEntity>();
            Assert.IsTrue(extendedClass.IsSaved);
        }
    }
}

