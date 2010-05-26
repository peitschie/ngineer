using System.Reflection;
namespace NGineer.BuildHelpers
{
    public interface IMemberSetter
    {
        bool IsForMember(MemberInfo member, IBuilder builder, BuildSession session);
        void Set(object obj, IBuilder builder, BuildSession session);
    }
}