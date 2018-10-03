using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class BarlineExpression : Expression
    {

        public override void Interpret(LilypondContext context)
        {
            context.Sequence.Symbols.Add(new Barline());
        }
    }
}
