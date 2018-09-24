using System;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class NoteExpression : Expression
    {

        private int _note;
        private int _octave;

        public NoteExpression(char note, int octave)
        {
            _note = LilypondSequenceReader.NotesOrder.IndexOf(note);
            _octave = octave;
        }

        public override void Interpret(LilypondContext context)
        {
            throw new NotImplementedException();
        }
    }
}
