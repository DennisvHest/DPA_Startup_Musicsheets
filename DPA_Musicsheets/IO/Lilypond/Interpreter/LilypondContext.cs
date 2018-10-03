using System.Collections.Generic;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class LilypondContext
    {
        public MusicalSequence Sequence { get; set; }
        public int RelativePitch { get; set; }
        public int CurrentOctave { get; set; }
        public int? PreviousNotePitch { get; set; }
        public bool ClefAdded { get; set; }

        public LilypondContext()
        {
            Sequence = new MusicalSequence
            {
                Symbols = new List<IMusicalSymbol>()
            };

            CurrentOctave = 4;
        }
    }
}
