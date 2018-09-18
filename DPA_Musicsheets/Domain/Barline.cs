namespace DPA_Musicsheets.Domain
{
    public class Barline : MusicalSymbol
    {
        public RepeatType RepeatType { get; set; }
        public int AlternateRepeatGroup { get; set; }
    }
}
