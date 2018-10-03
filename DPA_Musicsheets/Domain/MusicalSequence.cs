using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Domain
{
    public class MusicalSequence
    {
        public int BeatsPerMinute { get; set; }
        public List<IMusicalSymbol> Symbols { get; set; }

        public MusicalSequence()
        {
            Symbols = new List<IMusicalSymbol>();
        }
    }
}
