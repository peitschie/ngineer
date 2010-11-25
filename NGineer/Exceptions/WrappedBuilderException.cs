using System;
using NGineer.Utils;

namespace NGineer.Exceptions
{
    public class WrappedBuilderException : BuilderException
    {
        private readonly BuildSession _session;

        public WrappedBuilderException(BuildSession session, Exception innerException)
            : base(string.Format("{0}: {1}", session.ConstructedChainInfo(), innerException.Message), innerException)
        {
            _session = session;
        }

        public BuildSession Session { get { return _session; } }
    }
}

