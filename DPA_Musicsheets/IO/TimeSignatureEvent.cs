using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO
{
    public interface ITimeSignatureEvent
    {
        TimeSignature TimeSignature { get; }
    }
}
