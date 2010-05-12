namespace NGineer.Sequences
{
    public static class Sequence
    {
        public static SimpleIntSequence Int()
        {
            return new SimpleIntSequence(new Counter());
        }
    }
}