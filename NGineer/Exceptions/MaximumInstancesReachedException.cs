using System;
using System.Collections.Generic;
using System.Linq;
using NGineer.Utils;
using NGineer.Exceptions;
using NGineer.Internal;

namespace NGineer.Exceptions
{
    public class MaximumInstancesReachedException : BuilderException
    {
        private readonly BuildSession _session;
        private readonly int _maxInstances;
        private List<BuilderStatEntry> _stats;
        private string _statsTop10;

        public MaximumInstancesReachedException(int maxInstances, BuildSession session)
            : base(string.Format("Maximum number of new objects was exceeded at {0} objects: {1}", maxInstances,
            GenerateSummaryString(GenerateStats(session))))
        {
            _session = session;
            _maxInstances = maxInstances;
        }

        public string Top10Statistics
        {
            get
            {
                return _statsTop10 ?? (_statsTop10 = GenerateSummaryString(Statistics));
            }
        }

        public IList<BuilderStatEntry> Statistics
        {
            get { return _stats ?? (_stats = GenerateStats(Session)); }
        }

        public BuildSession Session
        {
            get { return _session; }
        }

        public int MaxInstances
        {
            get { return _maxInstances; }
        }

        private static string GenerateSummaryString(IEnumerable<BuilderStatEntry> statistics)
        {
            return string.Join(Environment.NewLine, statistics
                                                        .Where(e => BuilderInstanceTracker.IncludeInCount(e.Type))
                                                        .Take(10)
                                                        .Select(e => string.Format("{0}({1})", e.Type.Name, e.Count))
                                                        .ToArray());
        }

        private static List<BuilderStatEntry> GenerateStats(BuildSession session)
        {
            var stats = new Dictionary<Type, int>();
            GenerateStats(session.BuiltObjectTreeRoot, stats, new List<ObjectBuildRecord>());
            return stats.Select(e => new BuilderStatEntry(e.Key, e.Value))
				.OrderByDescending(s => s.Count)
				.ThenBy(s => s.Type.Name)
				.ToList();
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
            int existing;
            values.TryGetValue(key, out existing);
            values[key] = existing + 1;
        }
    }
}