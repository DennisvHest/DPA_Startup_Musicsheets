using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Domain
{
    public class Cross : NoteDecoration
    {
        public override int NoteAlteration => Note.NoteAlteration + 1;
    }
}
