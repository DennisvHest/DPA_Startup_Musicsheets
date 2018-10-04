namespace DPA_Musicsheets.Domain
{
    public abstract class NoteDecoration : INote
    {
        public INote Note { get; set; }

        public char NoteName => Note.NoteName;
        public virtual int NoteAlteration => 0;
        public int MidiPitch => Note.MidiPitch;
        public MusicalSymbolDuration Duration => Note.Duration;
        public virtual int Dots => 0;
        public NoteTieType NoteTieType => Note.NoteTieType;
    }
}
