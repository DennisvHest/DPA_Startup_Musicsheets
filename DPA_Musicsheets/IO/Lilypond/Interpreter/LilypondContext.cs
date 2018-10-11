using System.Collections.Generic;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class LilypondContext
    {
        public MusicalSequence Sequence { get; set; }
        public int CurrentOctave { get; set; }
        public int? PreviousNotePitch { get; set; }
        public NoteTieType PreviousTieType { get; set; }
        public bool ClefAdded { get; set; }
        public bool InRepeat { get; set; }
        public bool InAlternative { get; set; }
        public bool InAlternativeGroup { get; set; }
        public int CurrentAlternativeGroup { get; set; }

        public bool LinkingNote => PreviousTieType == NoteTieType.Start ||
                                   PreviousTieType == NoteTieType.StopAndStartAnother;

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
