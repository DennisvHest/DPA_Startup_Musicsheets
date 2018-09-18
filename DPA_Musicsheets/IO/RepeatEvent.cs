using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO
{
    public interface IRepeatEvent
    {
        Barline Barline { get; }
    }
}
