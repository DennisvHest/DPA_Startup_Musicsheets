using System;
using DPA_Musicsheets.Domain;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiTimeSignatureEvent : ITimeSignatureEvent
    {
        private readonly MetaMessage _midiTimeSignatureMessage;

        public TimeSignature TimeSignature
        {
            get
            {
                byte[] timeSignatureBytes = _midiTimeSignatureMessage.GetBytes();

                return new TimeSignature
                {
                    BeatUnit = timeSignatureBytes[0],
                    BeatsPerBar = (uint)(1 / Math.Pow(timeSignatureBytes[1], -2))
                };
            }
        }

        public MidiTimeSignatureEvent(MetaMessage midiTimeSignatureMessage)
        {
            _midiTimeSignatureMessage = midiTimeSignatureMessage;
        }
    }
}
