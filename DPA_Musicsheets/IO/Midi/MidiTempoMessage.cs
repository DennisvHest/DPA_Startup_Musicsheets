using System;
using DPA_Musicsheets.Domain;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiTempoMessage : MidiMessage
    {
        private readonly MetaMessage _beatsPerMinuteMessage;

        public MidiTempoMessage(MusicalSequence sequence, MetaMessage message) : base(sequence)
        {
            _beatsPerMinuteMessage = message;
        }

        public override void Parse()
        {
            byte[] tempoBytes = _beatsPerMinuteMessage.GetBytes();
            int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
            Sequence.BeatsPerMinute = 60000000 / tempo;
        }
    }
}
