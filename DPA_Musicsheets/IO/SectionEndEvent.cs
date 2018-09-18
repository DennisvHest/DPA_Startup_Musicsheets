using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO
{
    interface ISectionEndEvent
    {
        Barline Barline { get; }
    }
}
