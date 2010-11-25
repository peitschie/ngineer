using System;
using System.Text;

namespace NGineer.Exceptions
{
    public class DepthExceededException : BuilderException
    {
        private readonly int _depth;

        public DepthExceededException(int depth, BuildSession session)
            : base(string.Format("Maximum build depth of {0} was exceeded", depth), session)
        {
            _depth = depth;
        }

        public int Depth
        {
            get { return _depth; }
        }
    }
}