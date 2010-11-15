namespace NGineer.Exceptions
{
    public class BuilderSealedException : BuilderException
    {
        public BuilderSealedException() : base("Builder class has been sealed.  Please use CreateNew to create a child instance")
        {
        }
    }
}