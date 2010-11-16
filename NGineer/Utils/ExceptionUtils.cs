using System;
using System.Text;
using NGineer.Utils;
namespace NGineer.Utils
{
    public static class ExceptionUtils
    {
        public static string ConstructedChainInfo(this BuildSession session)
        {
            var result = new StringBuilder();
            var first = true;
            foreach (var current in session.CurrentMemberStack)
            {
                if (!first)
                {
                    result.Insert(0, "->");
                }
                first = false;
                result.Insert(0, string.Format("{0} {1}", current.ReturnType().Name, current.Name));
            }
            return result.ToString();
        }
    }
}

