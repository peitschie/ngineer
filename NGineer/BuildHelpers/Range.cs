namespace NGineer.BuildHelpers
{
    public class Range
    {
        private readonly int _minimum;
        private readonly int _maximum;

        public Range(int minimum, int maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        public int Minimum
        {
            get { return _minimum; }
        }

        public int Maximum
        {
            get { return _maximum; }
        }
    }
}