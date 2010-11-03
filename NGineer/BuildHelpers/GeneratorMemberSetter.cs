using System.Reflection;
using NGineer.Utils;
using System;
using NGineer.BuildGenerators;
using System.Linq;

namespace NGineer.BuildHelpers
{
    public class GeneratorMemberSetter : AbstractMemberSetter, IMemberSetter
    {
		private readonly IGenerator _generator;
		
        public GeneratorMemberSetter(MemberInfo member, Type declaringType, IGenerator generator, bool allowInherited)
			: base(member, declaringType, allowInherited)
        {
        	if (generator == null)
        		throw new ArgumentNullException("generator");
        	_generator = generator;
        }

        public bool IsForMember (MemberInfo member, IBuilder builder, BuildSession session)
        {
            return IsForMember(member) && _generator.GeneratesType(MemberReturnType, builder, session);
        }

        public void Set(object obj, IBuilder builder, BuildSession session)
        {
            var generatorValue = _generator.Create(MemberReturnType, builder, session);
            var populator = Builder.Populators.LastOrDefault(p => p.PopulatesType(MemberReturnType, builder, session));
            if(populator != null)
                populator.Populate(MemberReturnType, generatorValue, builder, session);
            Member.SetValue(obj, generatorValue);
        }
    }
}
