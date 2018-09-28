using System.Collections.Generic;
using System.Text.RegularExpressions;
using PSAMControlLibrary;

namespace DPA_Musicsheets.Managers
{
    public class PsamStaffBuilder : StaffBuilder
    {
        public override void AddSymbol(Domain.Clef clef)
        {
            Symbols.Add(new Clef((ClefType)clef.ClefType, 2));
        }

        public override void AddSymbol(Domain.TimeSignature timeSignature)
        {
            Symbols.Add(new TimeSignature(TimeSignatureType.Numbers, timeSignature.BeatsPerBar, timeSignature.BeatUnit));
        }

        public override void AddSymbol(Domain.Barline barLine)
        {
            Symbols.Add(new Barline());
        }

        public override void AddSymbol(Domain.Note note)
        {
            string noteName = MidiToLilyHelper.GetNoteName(note.MidiPitch);

            int alter = 0;
            alter += Regex.Matches(noteName, "is").Count;
            alter -= Regex.Matches(noteName, "es|as").Count;

            int octave = note.MidiPitch / 12 - 1;

            Symbols.Add(new Note(noteName[0].ToString().ToUpper(),
                alter, octave, (MusicalSymbolDuration)note.Duration, NoteStemDirection.Up,
                NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }));
        }
    }
}
