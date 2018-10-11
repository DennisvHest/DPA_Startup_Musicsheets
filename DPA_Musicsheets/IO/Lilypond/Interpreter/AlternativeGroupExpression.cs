using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class AlternativeGroupExpression : LilypondSection
    {
        public override void Interpret(LilypondContext context)
        {
            // The second alternative group has a backward repeat barline
            RepeatType repeatType = context.CurrentAlternativeGroup == 2 ? RepeatType.Backward : RepeatType.None;

            context.Sequence.Symbols.Add(new Barline
            {
                AlternateRepeatGroup = context.CurrentAlternativeGroup,
                RepeatType = repeatType
            });

            base.Interpret(context);

            context.CurrentAlternativeGroup++;
        }
    }
}
