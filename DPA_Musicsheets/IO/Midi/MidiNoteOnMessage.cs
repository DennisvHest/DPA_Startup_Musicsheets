using System;
using DPA_Musicsheets.Domain;
using DPA_Musicsheets.Managers;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiNoteOnMessage : MidiMessage
    {
        private readonly MidiEvent _noteOnEvent;
        private readonly Bar _currentBar;
        private Note _previousMidiNote;
        private bool _startedNoteIsClosed;
        private int _previousNoteAbsoluteTicks;
        private readonly int _division;
        private int percentageOfBarReached;
        private int _beatsPerBar;
        private int _beatNote;

        public MidiNoteOnMessage(
            MusicalSequence sequence,
            MidiEvent noteOnEvent, 
            Bar currentBar, 
            Note previousMidiNote, 
            bool startedNoteIsClosed,
            int previousNoteAbsoluteTicks,
            int division) : base(sequence)
        {
            _noteOnEvent = noteOnEvent;
            _currentBar = currentBar;
            _previousMidiNote = previousMidiNote;
            _startedNoteIsClosed = startedNoteIsClosed;
            _previousNoteAbsoluteTicks = previousNoteAbsoluteTicks;
            _division = division;
        }

        public override void Parse()
        {
            ChannelMessage noteOnMessage = _noteOnEvent.MidiMessage as ChannelMessage;

            if (noteOnMessage.Data2 > 0) // Data2 = loudness
            {
                Note note = new Note { MidiPitch = noteOnMessage.Data1 };

                _currentBar.MusicalSymbols.Add(note);

                _previousMidiNote = note;
                _startedNoteIsClosed = false;
            } else if (_startedNoteIsClosed)
            {
                // Finish the previous note with the length.
                double percentageOfBar;

                int duration = 0;
                int dots = 0;

                double deltaTicks = _noteOnEvent.AbsoluteTicks - _previousNoteAbsoluteTicks;

                if (deltaTicks <= 0)
                {
                    percentageOfBar = 0;
                }

                double percentageOfBeatNote = deltaTicks / _division;
                percentageOfBar = (1.0 / _beatsPerBar) * percentageOfBeatNote;

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
                            var addtime = 1 / ((subtractDuration / _beatNote) * Math.Pow(2, dots));
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

                _previousNoteAbsoluteTicks = _noteOnEvent.AbsoluteTicks;

                percentageOfBarReached += percentageOfBar;
                if (percentageOfBarReached >= 1)
                {
                    percentageOfBarReached -= 1;
                }

                _startedNoteIsClosed = true;
            }
        }
    }
}
