using System;

namespace NGineer.BuildHelpers
{
    public class BuildContext : IBuilder
    {
        private readonly int _maximumDepth;
        private readonly Func<Type, BuildContext, object> _buildAction;
        private int _currentLevel;

        public BuildContext(int maximumDepth, Func<Type, BuildContext, object> buildAction)
        {
            _maximumDepth = maximumDepth;
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
    }
}