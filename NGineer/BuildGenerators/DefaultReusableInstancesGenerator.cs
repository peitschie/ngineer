using System;
using System.Collections.Generic;
using System.Linq;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class DefaultReusableInstancesGenerator : IGenerator
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


        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            return session.Random.OneFromList(GetAllInstancesOf(session.ConstructedNodes, type));
        }
    }
}