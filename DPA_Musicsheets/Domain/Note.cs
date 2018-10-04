namespace DPA_Musicsheets.Domain
{
    public interface INote : IMusicalSymbol
    {
        char NoteName { get; }
        int NoteAlteration { get; }
        int MidiPitch { get; }
        MusicalSymbolDuration Duration { get; }
        int Dots { get; }
        NoteTieType NoteTieType { get; }
    }

    public class Note : MusicalSymbol, INote
    {
        public char NoteName { get; set; }
        public MusicalSymbolDuration Duration { get; set; }
        public int Dots => 0;
        public NoteTieType NoteTieType { get; set; }
        public int NoteAlteration => 0;
        public int MidiPitch { get; set; }
    }
}
