using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NGineer.BuildHelpers;

namespace NGineer.Generators
{
    public class ReusableInstancesGenerator : IGenerator
    {
        private readonly Random _random;
        private readonly Dictionary<Type, int> _numberOfInstances = new Dictionary<Type, int>();

        public ReusableInstancesGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public ReusableInstancesGenerator SetNumberOfInstances<TType>(int min, int max)
        {
            var type = typeof (TType);
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

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            var key = _numberOfInstances.Keys.FirstOrDefault(k => k.IsAssignableFrom(type));
            if(key != null)
            {
                var countOfObjs = GetAllInstancesOf(session.ConstructedObjects, type).Count();
                return countOfObjs >= _numberOfInstances[key];
            }
            return false;
        }

        private static IEnumerable<object> GetAllInstancesOf(IEnumerable<object> list, Type type)
        {
            return list.Where(o => o != null && type.IsAssignableFrom(o.GetType()));
        }


        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            return RandomHelpers.OneFromList(GetAllInstancesOf(session.ConstructedObjects, type), _random);
        }

        public object Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            return obj;
        }
    }
}