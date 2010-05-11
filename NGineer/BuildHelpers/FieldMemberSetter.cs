using System;
using System.Reflection;

namespace NGineer.BuildHelpers
{
    public class FieldMemberSetter : MemberSetter<FieldInfo>
    {
        private readonly FieldInfo _field;
        private readonly Func<object, IBuilder, BuildSession, object> _fieldValue;

        public FieldMemberSetter(FieldInfo field, Func<object, IBuilder, BuildSession, object> fieldValue)
        {
            if(field == null)
                throw new ArgumentNullException("field");
            _field = field;
            _fieldValue = fieldValue;
        }

        public override bool IsForMember(FieldInfo member)
        {
            return _field.Equals(member);
        }

        public override void Set(object obj, IBuilder builder, BuildSession session)
        {
            _field.SetValue(obj, _fieldValue(obj, builder, session));
        }
    }
}