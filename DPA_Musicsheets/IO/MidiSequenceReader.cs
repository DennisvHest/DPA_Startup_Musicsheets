using DPA_Musicsheets.Domain;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO
{
    public class MidiSequenceReader : SequenceReader
    {
        public MusicalSequence Sequence { get; set; }

        public MidiSequenceReader(string fileName)
        {
            Sequence midiSequence = new Sequence();
            midiSequence.Load(fileName);

            for (int trackNr = 0; trackNr < midiSequence.Count; trackNr++)
            {
                Track track = midiSequence[trackNr];

                foreach (var midiEvent in track.Iterator())
                {
                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    // TODO: Split this switch statements and create separate logic.
                    // We want to split this so that we can expand our functionality later with new keywords for example.
                    // Hint: Command pattern? Strategies? Factory method?
                    switch (midiMessage.MessageType)
                    {
                        case MessageType.Meta:
                            MetaMessage metaMessage = midiMessage as MetaMessage;
                            switch (metaMessage.MetaType)
                            {
                                case MetaType.TimeSignature:
                                    break;
                                case MetaType.Tempo:
                                    break;
                                case MetaType.EndOfTrack:
                                    break;
                                default: break;
                            }
                            break;
                        case MessageType.Channel:
                            ChannelMessage channelMessage = midiEvent.MidiMessage as ChannelMessage;
                            if (channelMessage.Command == ChannelCommand.NoteOn)
                            {
                                if (channelMessage.Data2 > 0) // Data2 = loudness
                                {
                                }
                                else if (!startedNoteIsClosed)
                                {
                                    startedNoteIsClosed = true;
                                }
                                else
                                {
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
