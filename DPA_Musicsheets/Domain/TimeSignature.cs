using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Domain
{
    public class TimeSignature : MusicalSymbol
    {
        public uint BeatsPerBar { get; set; }
        public uint BeatUnit { get; set; }

        public TimeSignature(uint beatUnit, uint beatsPerBar)
        {
            BeatsPerBar = beatsPerBar;
            BeatUnit = beatUnit;
        }
    }
}
