using NGineer.BuildSequences;

namespace NGineer
{
    public static class Sequences
    {
        public static ConsecutiveSequenceController<TType, int> SiblingConsecutiveInt<TType>()
        {
            return new ConsecutiveSequenceController<TType, int>((o, b, s) => s.CurrentObject.Parent, SequenceGenerators.IntSequenceGenerator);
        }

        public static ConsecutiveSequenceController<TType, int> SiblingConsecutiveInt<TType>(ParentTypeFetcher<TType> parentFetcher)
        {
            return new ConsecutiveSequenceController<TType, int>(parentFetcher, SequenceGenerators.IntSequenceGenerator);
        }
    }
}