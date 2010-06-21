using System;
using System.Text;
using NGineer.Utils;

namespace NGineer.BuildHelpers
{
    public class BuilderDepthExceededException : BuilderException
    {
        private readonly int _depth;
        private readonly BuildSession _session;

        public BuilderDepthExceededException(int depth, BuildSession session) : base("Maximum build depth of {0} was exceeded: {1}".With(depth, BuildChain(session)))
        {
            _depth = depth;
            _session = session;
        }

        private static string BuildChain(BuildSession session)
        {
            var result = new StringBuilder();
            var first = true;
            var current = session.CurrentObject;
            while (current != null)
            {
                if (current.Type != null)
                {
                    if (!first)
                    {
                        result.Insert(0, "->");
                    }
                    first = false;
                    result.Insert(0, current.Type.Name);
                }
                current = current.Parent;
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