using System;
using NGineer.Generators;

namespace NGineer.BuildHelpers
{
    public class BuildContext : IBuilder
    {
        private readonly int _maximumDepth;
        private readonly IBuilder _parent;
        private readonly Func<Type, BuildContext, object> _buildAction;
        private int _currentLevel;

        public BuildContext(int maximumDepth, IBuilder parent, Func<Type, BuildContext, object> buildAction)
        {
            _maximumDepth = maximumDepth;
            _parent = parent;
            _buildAction = buildAction;
        }

        public object Build(Type type)
        {
            if (_currentLevel == _maximumDepth)
                return null;

            _currentLevel++;
            var obj = _buildAction(type, this);
            _currentLevel--;
            return obj;
        }

        public IBuilder WithGenerator(IGenerator generator)
        {
            _parent.WithGenerator(generator);
            return this;
        }

        public IBuilder SetMaximumDepth(int depth)
        {
            _parent.SetMaximumDepth(depth);
            return this;
        }

        public IBuilder CreateNew()
        {
            return _parent.CreateNew();
        }

        public TType Build<TType>()
        {
            return (TType) Build(typeof (TType));
        }

        public IBuilder SetValuesFor<TType>(Action<TType> setter)
        {
            _parent.SetValuesFor(setter);
            return this;
        }

        public IBuilder SetValuesFor<TType>(Func<TType, TType> setter)
        {
            _parent.SetValuesFor(setter);
            return this;
        }

        public IBuilder SetValuesFor<TType>(Action<TType, IBuilder> setter)
        {
            _parent.SetValuesFor(setter);
            return this;
        }

        public IBuilder SetValuesFor<TType>(Func<TType, IBuilder, TType> setter)
        {
            _parent.SetValuesFor(setter);
            return this;
        }

        public IBuilder Seal()
        {
            _parent.Seal();
            return this;
        }
    }
}