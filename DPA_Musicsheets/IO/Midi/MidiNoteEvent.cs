using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Domain;
using DPA_Musicsheets.Managers;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.IO.Midi
{
    public class MidiNoteEvent : INoteEvent
    {
        private readonly ChannelMessage _noteOnMessage;

        public INote Note
        {
            get
            {
                if (_noteOnMessage.Data2 == 0) // Data2 = loudness
                    return null;

                int pitch = _noteOnMessage.Data1;
                int key = pitch % 12;
                int keyExclCross = key / 2;

                if (key >= 5)
                    keyExclCross++;

                Note note = new Note
                {
                    Pitch = pitch
                };

                if (key < 5 && key % 2 != 0 || key > 5 && key % 2 == 0)
                {
                    // Pitch is sharp
                    note.NoteName = SequenceReader.NotesOrder[keyExclCross - 1];
                    return new Cross { Note = note };
                }
                else
                {
                    note.NoteName = SequenceReader.NotesOrder[keyExclCross];
                    return note;
                }
            }
        }

        public MidiNoteEvent(ChannelMessage noteOnMessage)
        {
            _noteOnMessage = noteOnMessage;
        }
    }
}
