using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSAMControlLibrary;

namespace DPA_Musicsheets.Managers
{
    public abstract class StaffBuilder
    {
        public List<MusicalSymbol> Symbols { get; }

        protected StaffBuilder()
        {
            Symbols = new List<MusicalSymbol>();
        }

        public abstract void AddSymbol(Domain.TimeSignature timeSignature);
        public abstract void AddSymbol(Domain.Barline barLine);
        public abstract void AddSymbol(Domain.Note note);
    }
}
