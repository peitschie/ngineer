using System;

namespace NGineer.Utils
{
    public static class GenericAssignableFrom
    {
        /// <summary>
        /// Check if the caller's type is a generic version of this type.  type can be called
        /// with empty generic arguments (e.g., IList<>), and this method will fill out the appropriate type
        /// from the caller
        /// </summary>
        /// <param name="type"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsGenericAssignableFrom(this Type type, Type other)
        {
            if (other == null || type == null || !other.IsGenericType || !type.IsGenericType)
                return false;
            /* Construct the dummy type that will be compared.  This means the caller doesnt
             * need to know the generic arguments of the type to compare to
             */
            try
            {
                type = type.GetGenericTypeDefinition().MakeGenericType(other.GetGenericArguments());
                if (type.IsAssignableFrom(other))
                {
                    return true;
                }
            }
            catch (ArgumentException)
            { }
            return false;

        }
    }
}