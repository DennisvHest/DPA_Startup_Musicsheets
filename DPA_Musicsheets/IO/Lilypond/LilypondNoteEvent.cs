using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond
{
    public class LilypondNoteEvent : INoteEvent
    {
        public Note Note { get; }
    }
}
