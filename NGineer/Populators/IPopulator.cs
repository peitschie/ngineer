using System;
using NGineer.BuildHelpers;
namespace NGineer.Populators
{
    public interface IPopulator
    {
        bool PopulatesType(Type type, IBuilder builder, BuildSession session);
        void Populate(Type type, object obj, IBuilder builder, BuildSession session);
    }
}

