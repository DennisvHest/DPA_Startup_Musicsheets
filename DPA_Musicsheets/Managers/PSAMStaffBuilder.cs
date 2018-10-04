

using System.Collections.Generic;
using DPA_Musicsheets.Domain;
using PSAMControlLibrary;
using Barline = PSAMControlLibrary.Barline;
using Clef = PSAMControlLibrary.Clef;
using ClefType = PSAMControlLibrary.ClefType;
using MusicalSymbolDuration = PSAMControlLibrary.MusicalSymbolDuration;
using Note = PSAMControlLibrary.Note;
using NoteTieType = PSAMControlLibrary.NoteTieType;
using TimeSignature = PSAMControlLibrary.TimeSignature;

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
            Symbols.Add(new Barline { RepeatSign = (RepeatSignType)barLine.RepeatType, AlternateRepeatGroup = barLine.AlternateRepeatGroup });
        }

        public override void AddSymbol(INote note)
        {
            int octave = note.MidiPitch / 12 - 1;

            Note staffNote = new Note(note.NoteName.ToString().ToUpper(),
                note.NoteAlteration, octave, (MusicalSymbolDuration)note.Duration, NoteStemDirection.Up,
                (NoteTieType)note.NoteTieType, new List<NoteBeamType>() { NoteBeamType.Single });

            staffNote.NumberOfDots = note.Dots;

            Symbols.Add(staffNote);
        }

        public override void AddSymbol(Domain.Rest rest)
        {
            Symbols.Add(new PSAMControlLibrary.Rest((MusicalSymbolDuration)rest.Duration));
        }
    }
}
