using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO
{
    public interface SequenceReader
    {
        MusicalSequence Sequence { get; set; }
    }
}
