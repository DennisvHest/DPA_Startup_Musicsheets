using DPA_Musicsheets.Domain;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiSequenceReader : SequenceReader
    {
        public MusicalSequence Sequence { get; set; }

        public MidiSequenceReader(string fileName)
        {
            Sequence midiSequence = new Sequence();
            midiSequence.Load(fileName);

            MusicalSequence sequence = new MusicalSequence();

            foreach (Track track in midiSequence)
            {
                foreach (MidiEvent midiEvent in track.Iterator())
                {
                    Sanford.Multimedia.Midi.IMidiMessage midiMessage = midiEvent.MidiMessage;

                    IMidiMessage message = null;

                    switch (midiMessage.MessageType)
                    {
                        case MessageType.Meta:
                            MetaMessage metaMessage = midiMessage as MetaMessage;
                            switch (metaMessage.MetaType)
                            {
                                case MetaType.TimeSignature:
                                    message = new MidiTimeSignatureMessage(sequence, metaMessage);
                                    break;
                                case MetaType.Tempo:
                                    message = new MidiTempoMessage(sequence, metaMessage);
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
                            }
                            break;
                    }

                    message?.Parse();
                }
            }
        }
    }
}
