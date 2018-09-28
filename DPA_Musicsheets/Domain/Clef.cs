namespace DPA_Musicsheets.Domain
{
    public class Clef : MusicalSymbol
    {
        public ClefType ClefType { get; set; }

        public Clef(ClefType clefType)
        {
            ClefType = clefType;
        }
    }
}
