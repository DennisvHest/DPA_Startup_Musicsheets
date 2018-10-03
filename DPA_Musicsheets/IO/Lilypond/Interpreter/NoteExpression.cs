using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    public class NoteExpression : Expression
    {

        private readonly char _noteName;
        private readonly int _notePitch;
        private readonly int _octaveChange;
        private readonly int _length;
        private readonly List<INote> _noteProperties;

        public NoteExpression(string noteExpression)
        {
            _noteName = noteExpression[0];

            // Pitch
            _notePitch = LilypondSequenceReader.NotesOrder.IndexOf(_noteName) * 2;
            if (_notePitch >= 5) // No E#
            {
                _notePitch--;
            }

            // Note decorations (crosses, moles, dots)
            _noteProperties = new List<INote>();

            int crosses = Regex.Matches(noteExpression, "is").Count;

            for (int i = 0; i < crosses; i++)
                _noteProperties.Add(new Cross());

            int moles = Regex.Matches(noteExpression, "es|as").Count;

            for (int i = 0; i < moles; i++)
                _noteProperties.Add(new Mole());

            int dots = noteExpression.Count(c => c == '.');

            for (int i = 0; i < dots; i++)
                _noteProperties.Add(new Dot());

            // Octave changes
            _octaveChange += noteExpression.Count(c => c == '\'');
            _octaveChange -= noteExpression.Count(c => c == ',');

            // Note length
            _length = int.Parse(Regex.Match(noteExpression, @"\d+").Value);
        }

        public override void Interpret(LilypondContext context)
        {
            // Raise or lower the current octave based on the distance between the previous/current note
            if (context.PreviousNotePitch != null)
            {
                if (context.PreviousNotePitch - _notePitch > 5)
                {
                    context.CurrentOctave++;
                }
                else if (_notePitch - context.PreviousNotePitch > 5)
                {
                    context.CurrentOctave--;
                }
            }
                
            context.CurrentOctave += _octaveChange;

            // Link the note and the note decorations
            NoteDecoration previousDecoration = null;

            foreach (NoteDecoration decoration in _noteProperties)
            {
                if (previousDecoration != null)
                {
                    previousDecoration.Note = decoration;
                }
                else
                {
                    previousDecoration = decoration;
                }
            }

            INote note = new Note
            {
                NoteName = _noteName,
                MidiPitch = _notePitch + context.CurrentOctave * 12,
                Duration = (MusicalSymbolDuration) _length
            };

            _noteProperties.Add(note);

            if (previousDecoration != null)
            {
                previousDecoration.Note = note;
            }

            context.Sequence.Symbols.Add(_noteProperties.First());

            context.PreviousNotePitch = _notePitch;
        }
    }
}
