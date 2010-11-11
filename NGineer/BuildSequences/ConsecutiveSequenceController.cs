using System;
using System.Collections.Generic;
using NGineer.Internal;

namespace NGineer.BuildSequences
{
    public delegate ObjectBuildTreeEntry ParentTypeFetcher<TObjType>(TObjType obj, IBuilder builder, BuildSession session);

    public class ConsecutiveSequenceController<TObjType, TSeqType>
    {
        private readonly ParentTypeFetcher<TObjType> _fetchParent;
        private readonly ISequenceGenerator<TSeqType> _generator;
        private readonly Stack<NestedEntry> _knownParents = new Stack<NestedEntry>();

        public ConsecutiveSequenceController(ParentTypeFetcher<TObjType> fetchParent,
            ISequenceGenerator<TSeqType> generator)
        {
            _fetchParent = fetchParent;
            _generator = generator;
        }

        public TSeqType Next(TObjType obj, IBuilder builder, BuildSession session)
        {
            var parent = _fetchParent(obj, builder, session);
            NestedEntry entry;
            if (_knownParents.Count > 0)
            {
                // Invariant: There should only ever be one entry at any given depth
                entry = _knownParents.Peek();

                if (!ReferenceEquals(parent, entry.Parent))
                {
                    if (parent.Depth > entry.Parent.Depth)
                    {
                        // New parent that is deeper than the last one seen on the stack
                        // Restart the numbering
                        entry = new NestedEntry(parent) { LastValue = _generator.StartNewSequence() };
                    }
                    else if (parent.Depth <= entry.Parent.Depth)
                    {
                        // Parent or sibling of the last parent
                        // Need to return to the same depth as the existing parent
                        while (_knownParents.Count > 0 && parent.Depth < entry.Parent.Depth)
                        {
                            entry = _knownParents.Pop();
                        }
                        if (!ReferenceEquals(parent, entry.Parent))
                        {
                            if (_knownParents.Count > 0)
                            {
                                _knownParents.Pop();
                            }
                            entry = new NestedEntry(parent) { LastValue = _generator.StartNewSequence() };
                        }
                    }
                    _knownParents.Push(entry);
                }
                
            }
            else
            {
                entry = new NestedEntry(parent);
                _knownParents.Push(entry);
            }
            var nextInt = entry.LastValue;
            entry.LastValue = _generator.NextInSequence(entry.LastValue);
            return nextInt;
        }

        private class NestedEntry
        {
            public readonly ObjectBuildTreeEntry Parent;
            public TSeqType LastValue;

            public NestedEntry(ObjectBuildTreeEntry parent)
            {
                Parent = parent;
            }

        }
    }
}