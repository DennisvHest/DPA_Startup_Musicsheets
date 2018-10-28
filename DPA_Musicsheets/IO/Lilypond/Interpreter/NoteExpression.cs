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
        private readonly bool _linkingNote;
        private readonly List<INote> _noteProperties;

        public NoteExpression(string noteExpression)
        {
            _noteName = noteExpression[0];

            // Note/Rest length
            if (!int.TryParse(Regex.Match(noteExpression, @"\d+").Value, out _length))
            {
                // No duration specified. Default is a half note.
                _length = 2;
            }
            

            if (_noteName == 'r')
                return; // Note is a Rest, we don't have to parse pitch, decorations, etc.

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

            // Note linking
            _linkingNote = noteExpression.EndsWith("~");
        }

        public override void Interpret(LilypondContext context)
        {
            if (_noteName == 'r')
            {
                // This note is a rest
                context.Sequence.Symbols.Add(new Rest { Duration = (MusicalSymbolDuration)_length });
                return;
            }

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

            // Linking notes
            NoteTieType tieType = NoteTieType.None;

            if (context.LinkingNote && _linkingNote)
            {
                tieType = NoteTieType.StopAndStartAnother;
            } else if (context.LinkingNote)
            {
                tieType = NoteTieType.Stop;
            } else if (_linkingNote)
            {
                tieType = NoteTieType.Start;
            }

            context.PreviousTieType = tieType;

            INote note = new Note
            {
                NoteName = _noteName,
                Pitch = _notePitch + context.CurrentOctave * 12,
                Duration = (MusicalSymbolDuration)_length,
                NoteTieType = tieType
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
