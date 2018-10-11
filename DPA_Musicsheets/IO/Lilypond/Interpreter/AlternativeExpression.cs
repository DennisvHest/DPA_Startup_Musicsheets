using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class AlternativeExpression : LilypondSection
    {
        public override void Interpret(LilypondContext context)
        {
            context.CurrentAlternativeGroup = 1;

            base.Interpret(context);

            // Reset the alternative group count and end the last alternative group with a barline
            context.CurrentAlternativeGroup = 0;

            context.Sequence.Symbols.Add(new Barline());
        }
    }
}
