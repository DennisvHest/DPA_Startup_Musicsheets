using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO
{
    public interface ISectionStartEvent
    {
        Barline Barline { get; }
    }
}
