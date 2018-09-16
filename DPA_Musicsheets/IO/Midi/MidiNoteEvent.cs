using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Domain;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiNoteEvent : INoteEvent
    {
        private readonly ChannelMessage _noteOnMessage;

        public Note Note
        {
            get
            {
                if (_noteOnMessage.Data2 == 0) // Data2 = loudness
                    return null;

                return new Note { MidiPitch = _noteOnMessage.Data1 };
            }
        }

        public MidiNoteEvent(ChannelMessage noteOnMessage)
        {
            _noteOnMessage = noteOnMessage;
        }
    }
}
