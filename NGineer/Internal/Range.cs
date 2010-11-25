namespace NGineer.Internal
{
    public sealed class Range
    {
        private readonly int _minimum;
        private readonly int _maximum;

        public Range(int minimum, int maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        /// <summary>
        /// Inclusive lower bound of Range
        /// </summary>
        public int Minimum
        {
            get { return _minimum; }
        }

        /// <summary>
        /// Inclusive upper bound of Range
        /// </summary>
        public int Maximum
        {
            get { return _maximum; }
        }
    }
}