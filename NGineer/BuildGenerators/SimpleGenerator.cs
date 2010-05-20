﻿using System;
using NGineer.BuildHelpers;

namespace NGineer.BuildGenerators
{
    public abstract class SimpleGenerator<TType> : SingleTypeGenerator<TType>
    {
        protected readonly Random Random;

        protected SimpleGenerator(int seed)
        {
            Random = new Random(seed);
        }

        protected abstract TType Generate();

        public override void Populate(TType obj, IBuilder builder, BuildSession session)
        {
        }

        public override TType Create(Type type, IBuilder builder, BuildSession session)
        {
            return Generate();
        }
    }
}