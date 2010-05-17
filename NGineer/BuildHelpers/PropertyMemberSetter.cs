using System;
using System.Reflection;

namespace NGineer.BuildHelpers
{
    public class PropertyMemberSetter : MemberSetter<PropertyInfo>
    {
        protected readonly PropertyInfo Property;
        private readonly Func<object, IBuilder, BuildSession, object> _propertyValue;

        public PropertyMemberSetter(PropertyInfo property, Func<object, IBuilder, BuildSession, object> propertyValue)
        {
            Property = property;
            _propertyValue = propertyValue;
        }

        public override bool IsForMember(PropertyInfo property)
        {
            return property != null 
                && Property.ReflectedType.IsAssignableFrom(property.ReflectedType)
                && Equals(Property.DeclaringType, property.DeclaringType)
                && Equals(Property.PropertyType, property.PropertyType)
                && Equals(Property.Name, property.Name);
        }

        public override void Set(object obj, IBuilder builder, BuildSession session)
        {
            Property.SetValue(obj, _propertyValue(obj, builder, session), null);
        }
    }

    public class PropertyMemberSetter<TType> : PropertyMemberSetter
    {
        private readonly Func<TType, IBuilder, BuildSession, object> _propertyValue;

        public PropertyMemberSetter(PropertyInfo property, Func<TType, IBuilder, BuildSession, object> propertyValue)
            : base(property, null)
        {
            _propertyValue = propertyValue;
        }

        public override void Set(object obj, IBuilder builder, BuildSession session)
        {
            Property.SetValue(obj, _propertyValue((TType)obj, builder, session), null);
        }
    }
}