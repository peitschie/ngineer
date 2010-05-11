namespace NGineer.BuildHelpers
{
    public interface IMemberSetter
    {
        bool IsForMember(object member);
        void Set(object obj, IBuilder builder, BuildSession session);
    }

    public interface IMemberSetter<TType> : IMemberSetter
    {
        bool IsForMember(TType member);
    }
}