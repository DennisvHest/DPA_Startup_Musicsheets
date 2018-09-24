using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class LilypondContext
    {
        public int StartPitch { get; set; }
        public int octaveChange { get; set; }
    }
}
