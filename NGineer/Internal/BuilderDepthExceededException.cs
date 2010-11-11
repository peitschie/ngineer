using System;
using System.Text;
using NGineer.Utils;

namespace NGineer.Internal
{
    public class BuilderDepthExceededException : BuilderException
    {
        private readonly int _depth;
        private readonly BuildSession _session;

        public BuilderDepthExceededException(int depth, BuildSession session)
            : base(string.Format("Maximum build depth of {0} was exceeded: {1}", depth, BuildChain(session)))
        {
            _depth = depth;
            _session = session;
        }

        private static string BuildChain(BuildSession session)
        {
            var result = new StringBuilder();
            var first = true;
            foreach (var current in session.CurrentMemberStack)
            {
                if (!first)
                {
                    result.Insert(0, "->");
                }
                first = false;
                result.Insert(0, string.Format("{0} {1}", current.ReturnType().Name, current.Name));
            }
            return result.ToString();
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