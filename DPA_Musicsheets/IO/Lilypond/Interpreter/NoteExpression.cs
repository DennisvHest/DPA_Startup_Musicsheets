using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class NoteExpression : Expression
    {

        private readonly int _noteChar;
        private readonly int _notePitch;
        private readonly int _length;

        public NoteExpression(char note, int length)
        {
            _noteChar = note;
            _notePitch = LilypondSequenceReader.NotesOrder.IndexOf(note) * 2;
            if (_noteChar != 'f')
                _notePitch++;
            _length = length;
        }

        public override void Interpret(LilypondContext context)
        {
            if (context.PreviousNotePitch != null && context.PreviousNotePitch > _notePitch)
                context.CurrentOctave++;

            context.Sequence.Symbols.Add(new Note
            {
                MidiPitch = _notePitch + context.CurrentOctave * 12 - 1,
                Duration = (MusicalSymbolDuration)_length
            });

            context.PreviousNotePitch = _notePitch;
        }
    }
}
