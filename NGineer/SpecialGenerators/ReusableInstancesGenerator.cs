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
        private readonly Random _random;
        private readonly Dictionary<Type, int> _numberOfInstances = new Dictionary<Type, int>();

        public ReusableInstancesGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public ReusableInstancesGenerator SetNumberOfInstances(Type type, int min, int max)
        {
            var number = _random.Next(max - min) + min;
            if(_numberOfInstances.ContainsKey(type))
            {
                _numberOfInstances[type] = number;
            }
            else
            {
                _numberOfInstances.Add(type, number);
            }
            return this;
        }

        public ReusableInstancesGenerator SetNumberOfInstances<TType>(int min, int max)
        {
            return SetNumberOfInstances(typeof(TType), min, max);
        }

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            var key = _numberOfInstances.Keys.FirstOrDefault(k => k.IsAssignableFrom(type));
            if(key != null)
            {
                var countOfObjs = GetAllInstancesOf(session.ConstructedNodes, type).Count();
                return countOfObjs >= _numberOfInstances[key];
            }
            return false;
        }

        private static IEnumerable<ObjectBuildRecord> GetAllInstancesOf(IEnumerable<ObjectBuildTreeEntry> list, Type type)
        {
            return list.Where(o => o.Object != null && type.IsAssignableFrom(o.Object.GetType())).Select(o => o.Record);
        }


        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return RandomHelpers.OneFromList(GetAllInstancesOf(session.ConstructedNodes, type), _random);
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }
    }
}