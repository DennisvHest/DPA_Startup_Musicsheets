using System;
using System.Linq;
using System.Text.RegularExpressions;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class NoteExpression : Expression
    {

        private readonly int _notePitch;
        private readonly int _octaveChange;
        private readonly int _length;

        public NoteExpression(string noteExpression)
        {
            // Pitch
            _notePitch = LilypondSequenceReader.NotesOrder.IndexOf(noteExpression[0]) * 2;
            if (_notePitch >= 5) // No E#
            {
                _notePitch--;
            }

            // Crosses and moles
            _notePitch += Regex.Matches(noteExpression, "is").Count;
            _notePitch -= Regex.Matches(noteExpression, "es|as").Count;

            // Octave changes
            _octaveChange += noteExpression.Count(c => c == '\'');
            _octaveChange -= noteExpression.Count(c => c == ',');

            // Note length
            _length = int.Parse(Regex.Match(noteExpression, @"\d+").Value);
        }

        public override void Interpret(LilypondContext context)
        {
            if (context.PreviousNotePitch != null && context.PreviousNotePitch > _notePitch)
                context.CurrentOctave++;

            context.CurrentOctave += _octaveChange;

            context.Sequence.Symbols.Add(new Note
            {
                MidiPitch = _notePitch + context.CurrentOctave * 12,
                Duration = (MusicalSymbolDuration)_length
            });

            context.PreviousNotePitch = _notePitch;
        }
    }
}
