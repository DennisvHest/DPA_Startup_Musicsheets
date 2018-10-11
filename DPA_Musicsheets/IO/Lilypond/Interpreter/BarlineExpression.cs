using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class BarlineExpression : Expression
    {
        private readonly bool _repeat;

        public BarlineExpression() { }

        public BarlineExpression(bool repeat)
        {
            _repeat = repeat;
        }

        public override void Interpret(LilypondContext context)
        {
            context.Sequence.Symbols.Add(new Barline
            {
                RepeatType = _repeat ? RepeatType.Backward : RepeatType.None,
                AlternateRepeatGroup = context.CurrentAlternativeGroup
            });
        }
    }
}
