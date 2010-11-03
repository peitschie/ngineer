using System;
namespace NGineer.Populators
{
    public class ArrayPopulator : IPopulator
    {
        public bool PopulatesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsArray;
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            var arrayType = type.GetElementType();
            var array = (Array) obj;
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(builder.Build(arrayType, session), i);
            }
        }
    }
}

