namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class RelativeExpression : LilypondSection
    {
        private readonly int _startPitch;
        private readonly int _octaveChange;

        public RelativeExpression(char startPitch, string octaveChange)
        {
            _startPitch = LilypondSequenceReader.NotesOrder.IndexOf(startPitch) + 1;

            foreach (char character in octaveChange)
            {
                switch (character)
                {
                    case '\'': _octaveChange++; break;
                    case ',': _octaveChange--; break;
                }
            }
        }

        public override void Interpret(LilypondContext context)
        {
            context.PreviousNotePitch = _startPitch;
            context.CurrentOctave += _octaveChange;

            base.Interpret(context);
        }
    }
}
