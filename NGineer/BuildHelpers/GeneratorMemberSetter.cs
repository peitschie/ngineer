using System.Reflection;
using NGineer.Utils;
using System;
using NGineer.BuildGenerators;

namespace NGineer.BuildHelpers
{
    public class GeneratorMemberSetter : MemberSetter<object, object>
    {
        private readonly IGenerator _generator;

        public GeneratorMemberSetter(MemberInfo member, IGenerator generator)
            : base(member)
        {
            if(generator == null)
                throw new ArgumentNullException("generator");
            _generator = generator;
        }

        public override bool IsForMember (MemberInfo member, IBuilder builder, BuildSession session)
        {
            return base.IsForMember (member, builder, session)
                && _generator.GeneratesType(MemberReturnType, builder, session);
        }

        public override void Set (object obj, IBuilder builder, BuildSession session)
        {
            var generatorValue = _generator.Create(MemberReturnType, builder, session);
            _generator.Populate(MemberReturnType, generatorValue, builder, session);
            Member.SetValue(obj, generatorValue);
        }
    }
}
