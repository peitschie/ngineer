using NGineer.BuildSequences;

namespace NGineer
{
    public static class Sequences
    {
        public static ConsecutiveSequenceController<object, int> SiblingConsecutiveInt()
        {
            return new ConsecutiveSequenceController<object, int>((o, b, s) => s.CurrentObject.Parent, SequenceGenerators.IntSequenceGenerator);
        }

        public static ConsecutiveSequenceController<TType, int> SiblingConsecutiveInt<TType>(ParentTypeFetcher<TType> parentFetcher)
        {
            return new ConsecutiveSequenceController<TType, int>(parentFetcher, SequenceGenerators.IntSequenceGenerator);
        }
    }
}