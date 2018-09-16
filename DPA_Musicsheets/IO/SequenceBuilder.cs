using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO
{
    public abstract class SequenceBuilder
    {

        public MusicalSequence Sequence { get; }

        protected SequenceBuilder()
        {
            Sequence = new MusicalSequence();
        }
    }
}
