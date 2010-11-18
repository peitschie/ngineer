using System;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class ArrayGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsArray;
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            Array instance;
            if (session.AvailableBuildDepth >= 2)
            {
                var arrayType = type.GetElementType();
                var range = session.GetCollectionSize(arrayType);
                instance = Array.CreateInstance(arrayType, session.Random.NextInRange(range));
                for (int i = 0; i < instance.Length; i++)
                {
                    instance.SetValue(builder.Build(arrayType), i);
                }
            }
            else
            {
                instance = Array.CreateInstance(type.GetElementType(), 0);
            }
            return new ObjectBuildRecord(type, instance, false);
        }
    }
}