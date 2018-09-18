using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond
{
    public class LilypondRepeatEvent : IRepeatEvent
    {
        public Barline Barline { get; }

        public LilypondRepeatEvent()
        {
            Barline = new Barline { RepeatType = RepeatType.Forward };
        }
    }
}
