using NGineer.Utils;

namespace NGineer.BuildHelpers
{
    public class BuilderDepthExceededException : BuilderException
    {
        private readonly int _depth;
        private readonly BuildSession _session;

        public BuilderDepthExceededException(int depth, BuildSession session) : base("Maximum build depth of {0} was exceeded".With(depth))
        {
            _depth = depth;
            _session = session;
        }

        public int Depth
        {
            get { return _depth; }
        }

        public BuildSession Session
        {
            get { return _session; }
        }
    }
}