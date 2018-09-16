using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiTempoEvent : ITempoEvent
    {
        private readonly MetaMessage _midiTempoMessage;

        public int BeatsPerMinute
        {
            get
            {
                byte[] tempoBytes = _midiTempoMessage.GetBytes();
                int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
                return 60000000 / tempo;
            }
        }

        public MidiTempoEvent(MetaMessage midiTempoMessage)
        {
            _midiTempoMessage = midiTempoMessage;
        }
    }
}
