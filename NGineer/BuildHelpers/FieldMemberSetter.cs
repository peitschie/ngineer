using System;
using System.Reflection;

namespace NGineer.BuildHelpers
{
    public class FieldMemberSetter : MemberSetter<FieldInfo>
    {
        protected readonly FieldInfo Field;
        private readonly Func<object, IBuilder, BuildSession, object> _fieldValue;

        public FieldMemberSetter(FieldInfo field, Func<object, IBuilder, BuildSession, object> fieldValue)
        {
            if(field == null)
                throw new ArgumentNullException("field");
            Field = field;
            _fieldValue = fieldValue;
        }

        public override bool IsForMember(FieldInfo member)
        {
            return Field.Equals(member);
        }

        public override void Set(object obj, IBuilder builder, BuildSession session)
        {
            Field.SetValue(obj, _fieldValue(obj, builder, session));
        }
    }

    public class FieldMemberSetter<TType> : FieldMemberSetter
    {
        private readonly Func<TType, IBuilder, BuildSession, object> _fieldValue;

        public FieldMemberSetter(FieldInfo field, Func<TType, IBuilder, BuildSession, object> fieldValue)
            : base(field, null)
        {
            _fieldValue = fieldValue;
        }

        public override void Set(object obj, IBuilder builder, BuildSession session)
        {
            Field.SetValue(obj, _fieldValue((TType)obj, builder, session));
        }
    }
}