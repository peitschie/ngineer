using System;
namespace NGineer.Exceptions
{
    public class WrappedBuilderException : BuilderException
    {
        public WrappedBuilderException(BuildSession session, Exception innerException)
            : base("An exception occurred during building", session, innerException)
        {
        }
    }
}

