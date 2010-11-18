namespace NGineer.BuildSequences
{
    public static class SequenceGenerators
    {
        private static IntSequenceGenerator _intSequenceGenerator;

        public static IntSequenceGenerator IntSequenceGenerator { get { return _intSequenceGenerator ?? (_intSequenceGenerator = new IntSequenceGenerator()); } }
    }
}