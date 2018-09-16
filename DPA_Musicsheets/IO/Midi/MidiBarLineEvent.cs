using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Domain;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiBarLineEvent : IBarLineEvent
    {
        public Barline BarLine { get; }

        public MidiBarLineEvent(MidiEvent barLineEvent, Note previousNote, ref bool startedNoteIsClosed, ref int previousNoteAbsoluteTicks, int division, TimeSignature timeSignature,
            ref double percentageOfBarReached)
        {
            if (startedNoteIsClosed) return;

            // Finish the previous note with the length.
            previousNote.Duration = GetNoteLength(previousNoteAbsoluteTicks,
                barLineEvent.AbsoluteTicks, division, timeSignature.BeatUnit,
                timeSignature.BeatsPerBar, out double percentageOfBar);
            previousNoteAbsoluteTicks = barLineEvent.AbsoluteTicks;

            percentageOfBarReached += percentageOfBar;
            if (percentageOfBarReached >= 1)
            {
                BarLine = new Barline();
                percentageOfBarReached -= 1;
            }
            startedNoteIsClosed = true;
        }

        private MusicalSymbolDuration GetNoteLength(int absoluteTicks, int nextNoteAbsoluteTicks, int division, uint beatNote, uint beatsPerBar,
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
