using System.Collections.Generic;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO
{
    public abstract class SequenceReader
    {
        public static List<char> NotesOrder = new List<char> { 'c', 'd', 'e', 'f', 'g', 'a', 'b' };

        public MusicalSequence Sequence { get; set; }

        protected SequenceReader()
        {
            Sequence = new MusicalSequence();
        }
    }
}
