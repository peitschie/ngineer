using System;
using System.Text;
using NGineer.Utils;

namespace NGineer.Exceptions
{
    public class DepthExceededException : BuilderException
    {
        private readonly int _depth;
        private readonly BuildSession _session;

        public DepthExceededException(int depth, BuildSession session)
            : base(string.Format("Maximum build depth of {0} was exceeded: {1}", depth, session.ConstructedChainInfo()))
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