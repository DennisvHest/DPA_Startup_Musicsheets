using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class RepeatExpression : LilypondSection
    {
        public override void Interpret(LilypondContext context)
        {
            foreach (Expression expression in ChildExpressions)
            {
                expression.Interpret(context);
            }

            context.Sequence.Symbols.Add(new Barline { RepeatType = RepeatType.Backward });
        }
    }
}
