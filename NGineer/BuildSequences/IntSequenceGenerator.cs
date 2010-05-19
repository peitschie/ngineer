namespace NGineer.BuildSequences
{
    public class IntSequenceGenerator : ISequenceGenerator<int>
    {
        public int NextInSequence(int previous)
        {
            return previous + 1;
        }

        public int StartNewSequence()
        {
            return 0;
        }
    }
}