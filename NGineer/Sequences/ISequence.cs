using NGineer.BuildHelpers;

namespace NGineer.Sequences
{
    public interface ISequence<TObjType>
    {
        object Next(TObjType obj, IBuilder builder, BuildSession session);
    }
}