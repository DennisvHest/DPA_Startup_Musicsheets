namespace DPA_Musicsheets.Domain
{
    public abstract class MusicalSymbol
    {
        public MusicalSymbolDuration Duration { get; set; }

        protected MusicalSymbol() { }
    }
}
