﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Domain
{
    public class MusicalSequence
    {
        public TimeSignature TimeSignature { get; set; }
        public int BeatsPerMinute { get; set; }
        public IEnumerable<Bar> Bars { get; set; }
    }
}
