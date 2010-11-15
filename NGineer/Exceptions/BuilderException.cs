using System;

namespace NGineer.Exceptions
{
    public class BuilderException : Exception
    {
        public BuilderException(string s) : base(s)
        {
            
        }

        public BuilderException(string s, Exception innerException) : base(s, innerException)
        {

        }
    }
}