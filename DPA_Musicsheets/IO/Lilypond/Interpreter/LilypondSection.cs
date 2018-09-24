using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public abstract class LilypondSection : Expression
    {
        public List<Expression> ChildExpressions { get; set; }

        protected LilypondSection()
        {
            ChildExpressions = new List<Expression>();
        }
    }
}
