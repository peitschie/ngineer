using NGineer.Internal;

namespace NGineer
{
    public static class BuildHelpers
    {
        public static TType FindOrBuildParent<TType>(object obj, IBuilder builder, BuildSession session)
            where TType : class
        {
            return FindParent<TType>(obj, builder, session) ?? builder.Build<TType>();
        }

        public static TType FindParent<TType>(object obj, IBuilder builder, BuildSession session)
            where TType : class
        {
            if (session.CurrentObject.Parent == null)
                return default(TType);
            return FindParent<TType>(obj, builder, session.CurrentObject.Parent);
        }

        public static TType FindParent<TType>(object obj, IBuilder builder, ObjectBuildTreeEntry currentObj)
            where TType : class
        {
            if (currentObj.Parent == null)
                return default(TType);
            if (typeof(TType).IsAssignableFrom(currentObj.Type))
                return (TType)currentObj.Object;
            return FindParent<TType>(obj, builder, currentObj.Parent);
        }
    }
}