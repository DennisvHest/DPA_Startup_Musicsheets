namespace DPA_Musicsheets.Domain
{
    public class Note : MusicalSymbol
    {
        public int MidiPitch { get; set; }
        public bool Dotted { get; set; }
    }
}
