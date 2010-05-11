namespace NGineer.BuildHelpers
{
    public abstract class MemberSetter<TType> : IMemberSetter<TType> where TType : class
    {
        public bool IsForMember(object member)
        {
            return IsForMember(member as TType);
        }

        public abstract bool IsForMember(TType member);
        public abstract void Set(object obj, IBuilder builder, BuildSession session);
    }
}