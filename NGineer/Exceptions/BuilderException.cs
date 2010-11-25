using System;
using NGineer.Utils;

namespace NGineer.Exceptions
{
    public class BuilderException : Exception
    {
        private readonly BuildSession _session;

        public BuilderException(string s) : base(s)
        {
            
        }

        public BuilderException(string s, Exception innerException)
            : base(s, innerException)
        {

        }

        public BuilderException(string s, BuildSession session)
            : base(session != null ? string.Format("{0}: {1}", session.ConstructedChainInfo(), s) : s)
        {
            _session = session;
        }

        public BuilderException(string s, BuildSession session, Exception innerException)
            : base(session != null ? string.Format("{0}: {1}", session.ConstructedChainInfo(), s) : s, innerException)
        {
            _session = session;
        }

        public BuildSession Session { get { return _session; } }
    }
}