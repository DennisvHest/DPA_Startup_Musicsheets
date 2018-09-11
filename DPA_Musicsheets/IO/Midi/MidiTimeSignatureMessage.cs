using System;
using DPA_Musicsheets.Domain;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiTimeSignatureMessage : MidiMessage
    {
        private readonly MetaMessage _timeSignatureMessage;

        public MidiTimeSignatureMessage(MusicalSequence sequence, MetaMessage message) : base(sequence)
        {
            _timeSignatureMessage = message;
        }

        public override void Parse()
        {
            byte[] timeSignatureBytes = _timeSignatureMessage.GetBytes();
            Sequence.TimeSignature.BeatUnit = timeSignatureBytes[0];
            Sequence.TimeSignature.BeatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));
        }
    }
}
