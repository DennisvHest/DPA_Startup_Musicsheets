using System;
using DPA_Musicsheets.Domain;
using DPA_Musicsheets.Managers;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiNoteOnMessage : MidiMessage
    {
        private readonly ChannelMessage _noteOnMessage;

        public MidiNoteOnMessage(MusicalSequence sequence, ChannelMessage noteOnMessage) : base(sequence)
        {
            _noteOnMessage = noteOnMessage;
        }

        public override void Parse()
        {
            if (_noteOnMessage.Data2 > 0) // Data2 = loudness
            {
            }
        }
    }
}
