using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Midi
{
    public abstract class MidiMessage : IMidiMessage
    {
        protected readonly MusicalSequence Sequence;

        protected MidiMessage(MusicalSequence sequence)
        {
            Sequence = sequence;
        }

        public abstract void Parse();
    }
}
