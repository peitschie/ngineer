using System;
using System.Linq;
using NGineer.BuildGenerators;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class DefaultConstructorGenerator : IGenerator
    {
        private readonly Type _type;

        public DefaultConstructorGenerator() { }

        public DefaultConstructorGenerator(Type type)
        {
            _type = type;
        }

        private static object InvokeDefaultConstructor(Type type)
        {
            var constructor = type.GetConstructor(new Type[0]);
            return constructor != null ? constructor.Invoke(new object[0]) : null;
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return _type == null || Equals(_type, type);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            foreach (var member in session.CurrentObject.Record.UnconstructedMembers)
            {
                session.PushMember(member);
                member.SetValue(obj, builder.Build(member.ReturnType()));
                session.PopMember(true);
            }
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            object newObj = InvokeDefaultConstructor(type);
            if (newObj == null)
            {
                throw new BuilderException(string.Format("Unable to construct {0} as no default constructor was found", type));
            }
            return newObj;
        }
    }
}