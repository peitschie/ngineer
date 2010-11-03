using System;
using NGineer.Utils;
namespace NGineer.Populators
{
    public class DefaultPopulator : IPopulator
    {
        public bool PopulatesType(Type type, IBuilder builder, BuildSession session)
        {
            return true;
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
            if (obj == null)
                return;

            foreach (var member in session.CurrentObject.Record.UnconstructedMembers)
            {
                session.PushMember(member);
                member.SetValue(obj, builder.Build(member.ReturnType(), session));
                session.PopMember(true);
            }
        }
    }
}

