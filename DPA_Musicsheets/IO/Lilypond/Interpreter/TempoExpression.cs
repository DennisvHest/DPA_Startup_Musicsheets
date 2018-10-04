using System.Linq;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class TempoExpression : Expression
    {
        private readonly int _bpm;

        public TempoExpression(string expression)
        {
            _bpm = int.Parse(expression.Split('=').Last());
        }

        public override void Interpret(LilypondContext context)
        {
            context.Sequence.BeatsPerMinute = _bpm;
        }
    }
}
