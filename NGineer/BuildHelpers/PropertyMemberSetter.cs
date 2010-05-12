using System;
using System.Reflection;

namespace NGineer.BuildHelpers
{
    public class PropertyMemberSetter : MemberSetter<PropertyInfo>
    {
        private readonly PropertyInfo _property;
        private readonly Func<object, IBuilder, BuildSession, object> _propertyValue;

        public PropertyMemberSetter(PropertyInfo property, Func<object, IBuilder, BuildSession, object> propertyValue)
        {
            _property = property;
            _propertyValue = propertyValue;
        }

        public override bool IsForMember(PropertyInfo property)
        {
            return property != null 
                && Equals(_property.DeclaringType, property.DeclaringType)
                && Equals(_property.PropertyType, property.PropertyType)
                && Equals(_property.Name, property.Name);
        }

        public override void Set(object obj, IBuilder builder, BuildSession session)
        {
            _property.SetValue(obj, _propertyValue(obj, builder, session), null);
        }
    }
}