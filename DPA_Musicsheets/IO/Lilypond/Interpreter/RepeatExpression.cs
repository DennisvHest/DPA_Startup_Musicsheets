using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class RepeatExpression : LilypondSection
    {
        public override void Interpret(LilypondContext context)
        {
            context.InRepeat = true;

            context.Sequence.Symbols.Add(new Barline() { RepeatType = RepeatType.Forward });

            base.Interpret(context);
        }
    }
}
