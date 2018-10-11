using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class LilypondSection : Expression
    {
        public List<Expression> ChildExpressions { get; set; }

        public LilypondSection()
        {
            ChildExpressions = new List<Expression>();
        }

        public override void Interpret(LilypondContext context)
        {
            foreach (Expression expression in ChildExpressions)
            {
                expression.Interpret(context);
            }
        }
    }
}
