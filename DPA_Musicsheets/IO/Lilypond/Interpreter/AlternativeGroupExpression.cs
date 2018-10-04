using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class AlternativeGroupExpression : LilypondSection
    {
        public override void Interpret(LilypondContext context)
        {
            foreach (Expression expression in ChildExpressions)
            {
                expression.Interpret(context);
            }

            context.Sequence.Symbols.Add(new Barline());
        }
    }
}
