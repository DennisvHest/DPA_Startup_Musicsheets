using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class ClefExpression : Expression
    {
        private readonly ClefType _clefType;

        public ClefExpression(string clef)
        {
            switch (clef)
            {
                case "treble":
                    _clefType = ClefType.GClef; break;
                case "bass":
                    _clefType = ClefType.FClef; break;
                default:
                    _clefType = ClefType.CClef; break;
            }
        }

        public override void Interpret(LilypondContext context)
        {
            context.Sequence.Symbols.Add(new Clef(_clefType));
            context.ClefAdded = true;
        }
    }
}
