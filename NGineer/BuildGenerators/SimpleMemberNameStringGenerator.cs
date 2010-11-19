using System;
using System.Collections.Generic;
using NGineer.Internal;

namespace NGineer.BuildGenerators
{
    public class SimpleMemberNameStringGenerator : IGenerator
    {
        private readonly Dictionary<string, int> _memberCounts = new Dictionary<string, int>();

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.Equals(typeof(string));
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            var key = session.CurrentMember != null ? session.CurrentMember.Name : type.Name;
            int lastCount;
            if (!_memberCounts.TryGetValue(key, out lastCount))
            {
                lastCount = 0;
                _memberCounts.Add(key, lastCount);
            }
            var newString = key + (lastCount);
            _memberCounts[key] = lastCount + 1;
            return new ObjectBuildRecord(type, newString, false);
        }
    }
}