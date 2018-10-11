using System.Linq;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class TimeSignatureExpression : Expression
    {
        private readonly uint _beatUnit;
        private readonly uint _beatsPerBar;

        public TimeSignatureExpression(string timeSignatureExpression)
        {
            uint[] timeSignature = timeSignatureExpression.Split('/').Select(uint.Parse).ToArray();

            _beatUnit = timeSignature[1];
            _beatsPerBar = timeSignature[0];
        }

        public override void Interpret(LilypondContext context)
        {
            context.Sequence.Symbols.Add(new TimeSignature(_beatUnit, _beatsPerBar));
        }
    }
}
