using System;

namespace NGineer.BuildSequences
{
    public interface ISequenceGenerator<TSeqType>
    {
        TSeqType NextInSequence(TSeqType previous);
        TSeqType StartNewSequence();
    }
}