using System.Collections.Generic;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.Managers
{
    public abstract class StaffBuilder
    {
        public List<PSAMControlLibrary.MusicalSymbol> Symbols { get; }

        protected StaffBuilder()
        {
            Symbols = new List<PSAMControlLibrary.MusicalSymbol>();
        }

        public abstract void AddSymbol(Clef clef);
        public abstract void AddSymbol(TimeSignature timeSignature);
        public abstract void AddSymbol(Barline barLine);
        public abstract void AddSymbol(INote note);
        public abstract void AddSymbol(Rest rest);
    }
}
