using System;
using System.Collections.Generic;
using System.Linq;
using NGineer.Utils;

namespace NGineer.BuildHelpers
{
    public class BuilderMaximumInstancesReached : BuilderException
    {
        private readonly BuildSession _session;
        private readonly int _maxInstances;
        private List<BuilderStatEntry> _stats;
        private string _statsTop10;

        public BuilderMaximumInstancesReached(int maxInstances, BuildSession session)
            : base("Maximum number of new objects was exceeded at {0} objects".With(maxInstances))
        {
            _session = session;
            _maxInstances = maxInstances;
        }

        public string Top10Statistics
        {
            get
            {
                return _statsTop10 ?? (_statsTop10 = string.Join(Environment.NewLine, Statistics
                    .Where(e => BuilderInstanceTracker.IncludeInCount(e.Type))
                    .Take(10)
                    .Select(e => "{0}({1})".With(e.Type.Name, e.Count))
                    .ToArray()));
            }
        }

        public IList<BuilderStatEntry> Statistics
        {
            get { return _stats ?? (_stats = GenerateStats()); }
        }

        public BuildSession Session
        {
            get { return _session; }
        }

        public int MaxInstances
        {
            get { return _maxInstances; }
        }

        private List<BuilderStatEntry> GenerateStats()
        {
            var stats = new Dictionary<Type, int>();
            GenerateStats(_session.BuiltObjectTreeRoot, stats, new List<ObjectBuildRecord>());
            return stats.Select(e => new BuilderStatEntry(e.Key, e.Value)).OrderByDescending(s => s.Count).ToList();
        }

        private static void GenerateStats(ObjectBuildTreeEntry node, IDictionary<Type, int> stats, IList<ObjectBuildRecord> seen)
        {
            foreach (var child in node.Children.Where(c => c.Type != null && !seen.Contains(c.Record)))
            {
                seen.Add(child.Record);
                AddOrIncrement(child.Type, stats);
                GenerateStats(child, stats, seen);
            }
        }

        private static void AddOrIncrement<TKey>(TKey key, IDictionary<TKey, int> values)
        {
            int existing = 0;
            values.TryGetValue(key, out existing);
            values[key] = existing + 1;
        }
    }
}