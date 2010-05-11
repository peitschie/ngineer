namespace NGineer.BuildHelpers
{
    public interface IMemberSetter
    {
        bool IsForMember(object member);
        void Set(object obj, IBuilder builder, BuildSession session);
    }

    public interface IMemberSetter<in TType> : IMemberSetter
    {
        bool IsForMember(TType member);
    }
}