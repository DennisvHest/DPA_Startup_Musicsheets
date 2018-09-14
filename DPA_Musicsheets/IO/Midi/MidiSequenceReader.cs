using System;
using System.Text;
using DPA_Musicsheets.Domain;
using DPA_Musicsheets.Managers;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiSequenceReader : SequenceReader
    {
        public MidiSequenceReader(string fileName)
        {
            Sequence midiSequence = new Sequence();
            midiSequence.Load(fileName);

            int division = midiSequence.Division;
            Note previousNote = null;
            int previousNoteAbsoluteTicks = 0;
            double percentageOfBarReached = 0;
            bool startedNoteIsClosed = true;
            TimeSignature previousTimeSignature = null;

            foreach (Track track in midiSequence)
            {
                foreach (var midiEvent in track.Iterator())
                {
                    Sanford.Multimedia.Midi.IMidiMessage midiMessage = midiEvent.MidiMessage;
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
                                    byte[] timeSignatureBytes = metaMessage.GetBytes();

                                    TimeSignature timeSignature = new TimeSignature
                                    {
                                        BeatUnit = timeSignatureBytes[0],
                                        BeatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2))
                                    };

                                    previousTimeSignature = timeSignature;

                                    Sequence.Symbols.Add(timeSignature);
                                    break;
                                case MetaType.Tempo:
                                    byte[] tempoBytes = metaMessage.GetBytes();
                                    int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
                                    Sequence.BeatsPerMinute = 60000000 / tempo;
                                    break;
                                case MetaType.EndOfTrack:
                                    break;
                                default: break;
                            }
                            break;
                        case MessageType.Channel:
                            var channelMessage = midiEvent.MidiMessage as ChannelMessage;
                            if (channelMessage.Command == ChannelCommand.NoteOn)
                            {
                                if (channelMessage.Data2 > 0) // Data2 = loudness
                                {
                                    Note note = new Note
                                    {
                                        MidiPitch = channelMessage.Data1
                                    };

                                    Sequence.Symbols.Add(note);

                                    previousNote = note;
                                    startedNoteIsClosed = false;
                                }
                                else if (!startedNoteIsClosed)
                                {
                                    // Finish the previous note with the length.
                                    double percentageOfBar;
                                    previousNote.Duration = GetNoteLength(previousNoteAbsoluteTicks,
                                        midiEvent.AbsoluteTicks, division, previousTimeSignature.BeatUnit,
                                        previousTimeSignature.BeatsPerBar, out percentageOfBar);
                                    previousNoteAbsoluteTicks = midiEvent.AbsoluteTicks;

                                    percentageOfBarReached += percentageOfBar;
                                    if (percentageOfBarReached >= 1)
                                    {
                                        Sequence.Symbols.Add(new Barline());
                                        percentageOfBarReached -= 1;
                                    }
                                    startedNoteIsClosed = true;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private MusicalSymbolDuration GetNoteLength(int absoluteTicks, int nextNoteAbsoluteTicks, int division, int beatNote, int beatsPerBar,
            out double percentageOfBar)
        {
            int duration = 0;
            int dots = 0;

            double deltaTicks = nextNoteAbsoluteTicks - absoluteTicks;

            if (deltaTicks <= 0)
            {
                percentageOfBar = 0;
                return 0;
            }

            double percentageOfBeatNote = deltaTicks / division;
            percentageOfBar = (1.0 / beatsPerBar) * percentageOfBeatNote;

            for (int noteLength = 32; noteLength >= 1; noteLength -= 1)
            {
                double absoluteNoteLength = (1.0 / noteLength);

                if (percentageOfBar <= absoluteNoteLength)
                {
                    if (noteLength < 2)
                        noteLength = 2;

                    int subtractDuration;

                    if (noteLength == 32)
                        subtractDuration = 32;
                    else if (noteLength >= 16)
                        subtractDuration = 16;
                    else if (noteLength >= 8)
                        subtractDuration = 8;
                    else if (noteLength >= 4)
                        subtractDuration = 4;
                    else
                        subtractDuration = 2;

                    if (noteLength >= 17)
                        duration = 32;
                    else if (noteLength >= 9)
                        duration = 16;
                    else if (noteLength >= 5)
                        duration = 8;
                    else if (noteLength >= 3)
                        duration = 4;
                    else
                        duration = 2;

                    double currentTime = 0;

                    while (currentTime < (noteLength - subtractDuration))
                    {
                        var addtime = 1 / ((subtractDuration / beatNote) * Math.Pow(2, dots));
                        if (addtime <= 0) break;
                        currentTime += addtime;
                        if (currentTime <= (noteLength - subtractDuration))
                        {
                            dots++;
                        }
                        if (dots >= 4) break;
                    }

                    break;
                }
            }

            return (MusicalSymbolDuration)duration;
        }
    }
}
