using System;
using System.Collections.Generic;
using System.Linq;
using NGineer.BuildGenerators;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.SpecialGenerators
{
    public class ReusableInstancesGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            var maxInstances = session.GetMaximumNumberOfInstances(type);
            if(maxInstances != null)
            {
                var countOfObjs = GetAllInstancesOf(session.ConstructedNodes, type).Count();
                return countOfObjs >= maxInstances.Value;
            }
            return false;
        }

        private static IEnumerable<ObjectBuildRecord> GetAllInstancesOf(IEnumerable<ObjectBuildTreeEntry> list, Type type)
        {
            return list.Where(o => o.Object != null && type.IsAssignableFrom(o.Object.GetType()))
				.Select(o => o.Record);
        }


        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return RandomHelpers.OneFromList(GetAllInstancesOf(session.ConstructedNodes, type), session.Random);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }
    }
}