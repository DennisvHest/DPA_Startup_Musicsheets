using DPA_Musicsheets.ViewModels;
using PSAMControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DPA_Musicsheets.Domain;
using DPA_Musicsheets.IO;
using DPA_Musicsheets.IO.Lilypond;
using DPA_Musicsheets.IO.Midi;
using IMidiMessage = Sanford.Multimedia.Midi.IMidiMessage;
using MusicalSymbol = PSAMControlLibrary.MusicalSymbol;
using Note = PSAMControlLibrary.Note;

namespace DPA_Musicsheets.Managers
{
    /// <summary>
    /// This is the one and only god class in the application.
    /// It knows all about all file types, knows every viewmodel and contains all logic.
    /// TODO: Clean this class up.
    /// </summary>
    public class MusicLoader
    {
        #region Properties
        public List<MusicalSymbol> WPFStaffs { get; set; } = new List<MusicalSymbol>();

        public Sequence MidiSequence { get; set; }
        #endregion Properties

        private int _beatNote = 4;    // De waarde van een beatnote.
        private int _bpm = 120;       // Aantal beatnotes per minute.
        private int _beatsPerBar;     // Aantal beatnotes per maat.

        public LilypondViewModel LilypondViewModel { get; set; }
        public MidiPlayerViewModel MidiPlayerViewModel { get; set; }
        public StaffsViewModel StaffsViewModel { get; set; }
        public MusicalSequence Sequence { get; set; }

        /// <summary>
        /// Opens a file.
        /// TODO: Remove the switch cases and delegate.
        /// TODO: Remove the knowledge of filetypes. What if we want to support MusicXML later?
        /// TODO: Remove the calling of the outer viewmodel layer. We want to be able reuse this in an ASP.NET Core application for example.
        /// </summary>
        /// <param name="fileName"></param>
        public void OpenFile(string fileName)
        {
            if (Path.GetExtension(fileName).EndsWith(".mid"))
            {
                SequenceReader midiReader = new MidiSequenceReader(fileName);
                Sequence = midiReader.Sequence;

                MidiSequence = new Sequence();
                MidiSequence.Load(fileName);

                LilypondViewModel.LilypondTextLoaded(LoadMidiIntoLilypond(MidiSequence));
            }
            else if (Path.GetExtension(fileName).EndsWith(".ly"))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var line in File.ReadAllLines(fileName))
                {
                    sb.AppendLine(line);
                }

                LoadLilyPond(sb.ToString());

                LilypondViewModel.LilypondTextLoaded(sb.ToString());
            }
            else
            {
                throw new NotSupportedException($"File extension {Path.GetExtension(fileName)} is not supported.");
            }

            LoadStaffsAndMidi();
        }



        /// <summary>
        /// This creates WPF staffs and MIDI from Lilypond.
        /// </summary>
        /// <param name="content"></param>
        public void LoadStaffsAndMidi()
        {
            WPFStaffs.Clear();

            StaffBuilder staffBuilder = new PsamStaffBuilder();
            StaffLoader staffLoader = new StaffLoader(staffBuilder);

            staffLoader.LoadStaffs(Sequence.Symbols);

            WPFStaffs.AddRange(staffLoader.Symbols);
            this.StaffsViewModel.SetStaffs(this.WPFStaffs);

            MidiSequence = GetSequenceFromWPFStaffs();
            MidiPlayerViewModel.MidiSequence = MidiSequence;
        }

        public void LoadLilyPond(string lilyContent)
        {
            try
            {
                SequenceReader lilyReader = new LilypondSequenceReader(lilyContent);
                Sequence = lilyReader.Sequence;
                LoadStaffsAndMidi();
            }
            catch (Exception){ }            
        }

        #region Midi loading (loads midi to lilypond)

        /// <summary>
        /// TODO: Create our own domain classes to be independent of external libraries/languages.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public string LoadMidiIntoLilypond(Sequence sequence)
        {
            StringBuilder lilypondContent = new StringBuilder();
            lilypondContent.AppendLine("\\relative c' {");
            lilypondContent.AppendLine("\\clef treble");

            int division = sequence.Division;
            int previousMidiKey = 60; // Central C;
            int previousNoteAbsoluteTicks = 0;
            double percentageOfBarReached = 0;
            bool startedNoteIsClosed = true;

            for (int i = 0; i < sequence.Count(); i++)
            {
                Track track = sequence[i];

                foreach (var midiEvent in track.Iterator())
                {
                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    // TODO: Split this switch statements and create separate logic.
                    // We want to split this so that we can expand our functionality later with new keywords for example.
                    // Hint: Command pattern? Strategies? Factory method?
                    switch (midiMessage.MessageType)
                    {
                        case MessageType.Meta:
                            var metaMessage = midiMessage as MetaMessage;
                            switch (metaMessage.MetaType)
                            {
                                case MetaType.TimeSignature:
                                    byte[] timeSignatureBytes = metaMessage.GetBytes();
                                    _beatNote = timeSignatureBytes[0];
                                    _beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));
                                    lilypondContent.AppendLine($"\\time {_beatNote}/{_beatsPerBar}");
                                    break;
                                case MetaType.Tempo:
                                    byte[] tempoBytes = metaMessage.GetBytes();
                                    int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
                                    _bpm = 60000000 / tempo;
                                    lilypondContent.AppendLine($"\\tempo 4={_bpm}");
                                    break;
                                case MetaType.EndOfTrack:
                                    if (previousNoteAbsoluteTicks > 0)
                                    {
                                        // Finish the last notelength.
                                        double percentageOfBar;
                                        lilypondContent.Append(MidiToLilyHelper.GetLilypondNoteLength(previousNoteAbsoluteTicks, midiEvent.AbsoluteTicks, division, _beatNote, _beatsPerBar, out percentageOfBar));
                                        lilypondContent.Append(" ");

                                        percentageOfBarReached += percentageOfBar;
                                        if (percentageOfBarReached >= 1)
                                        {
                                            lilypondContent.AppendLine("|");
                                            percentageOfBar = percentageOfBar - 1;
                                        }
                                    }
                                    break;
                                default: break;
                            }
                            break;
                        case MessageType.Channel:
                            var channelMessage = midiEvent.MidiMessage as ChannelMessage;
                            if (channelMessage.Command == ChannelCommand.NoteOn)
                            {
                                if(channelMessage.Data2 > 0) // Data2 = loudness
                                {
                                    // Append the new note.
                                    lilypondContent.Append(MidiToLilyHelper.GetLilyNoteName(previousMidiKey, channelMessage.Data1));
                                    
                                    previousMidiKey = channelMessage.Data1;
                                    startedNoteIsClosed = false;
                                }
                                else if (!startedNoteIsClosed)
                                {
                                    // Finish the previous note with the length.
                                    double percentageOfBar;
                                    lilypondContent.Append(MidiToLilyHelper.GetLilypondNoteLength(previousNoteAbsoluteTicks, midiEvent.AbsoluteTicks, division, _beatNote, _beatsPerBar, out percentageOfBar));
                                    previousNoteAbsoluteTicks = midiEvent.AbsoluteTicks;
                                    lilypondContent.Append(" ");

                                    percentageOfBarReached += percentageOfBar;
                                    if (percentageOfBarReached >= 1)
                                    {
                                        lilypondContent.AppendLine("|");
                                        percentageOfBarReached -= 1;
                                    }
                                    startedNoteIsClosed = true;
                                }
                                else
                                {
                                    lilypondContent.Append("r");
                                }
                            }
                            break;
                    }
                }
            }

            lilypondContent.Append("}");

            return lilypondContent.ToString();
        }

        #endregion Midiloading (loads midi to lilypond)

        #region Saving to files
        internal void SaveToMidi(string fileName)
        {
            Sequence sequence = GetSequenceFromWPFStaffs();

            sequence.Save(fileName);
        }
        
        /// <summary>
        /// We create MIDI from WPF staffs, 2 different dependencies, not a good practice.
        /// TODO: Create MIDI from our own domain classes.
        /// TODO: Our code doesn't support repeats (rendering notes multiple times) in midi yet. Maybe with a COMPOSITE this will be easier?
        /// </summary>
        /// <returns></returns>
        private Sequence GetSequenceFromWPFStaffs()
        {
            List<string> notesOrderWithCrosses = new List<string>() { "c", "cis", "d", "dis", "e", "f", "fis", "g", "gis", "a", "ais", "b" };
            int absoluteTicks = 0;

            Sequence sequence = new Sequence();

            Track metaTrack = new Track();
            sequence.Add(metaTrack);

            // Calculate tempo
            int speed = (60000000 / _bpm);
            byte[] tempo = new byte[3];
            tempo[0] = (byte)((speed >> 16) & 0xff);
            tempo[1] = (byte)((speed >> 8) & 0xff);
            tempo[2] = (byte)(speed & 0xff);
            metaTrack.Insert(0 /* Insert at 0 ticks*/, new MetaMessage(MetaType.Tempo, tempo));

            Track notesTrack = new Track();
            sequence.Add(notesTrack);

            for (int i = 0; i < WPFStaffs.Count; i++)
            {
                var musicalSymbol = WPFStaffs[i];
                switch (musicalSymbol.Type)
                {
                    case MusicalSymbolType.Note:
                        Note note = musicalSymbol as Note;

                        // Calculate duration
                        double absoluteLength = 1.0 / (double)note.Duration;
                        absoluteLength += (absoluteLength / 2.0) * note.NumberOfDots;

                        double relationToQuartNote = _beatNote / 4.0;
                        double percentageOfBeatNote = (1.0 / _beatNote) / absoluteLength;
                        double deltaTicks = (sequence.Division / relationToQuartNote) / percentageOfBeatNote;

                        // Calculate height
                        int noteHeight = notesOrderWithCrosses.IndexOf(note.Step.ToLower()) + ((note.Octave + 1) * 12);
                        noteHeight += note.Alter;
                        notesTrack.Insert(absoluteTicks, new ChannelMessage(ChannelCommand.NoteOn, 1, noteHeight, 90)); // Data2 = volume

                        absoluteTicks += (int)deltaTicks;
                        notesTrack.Insert(absoluteTicks, new ChannelMessage(ChannelCommand.NoteOn, 1, noteHeight, 0)); // Data2 = volume

                        break;
                    case MusicalSymbolType.TimeSignature:
                        byte[] timeSignature = new byte[4];
                        timeSignature[0] = (byte)_beatsPerBar;
                        timeSignature[1] = (byte)(Math.Log(_beatNote) / Math.Log(2));
                        metaTrack.Insert(absoluteTicks, new MetaMessage(MetaType.TimeSignature, timeSignature));
                        break;
                    default:
                        break;
                }
            }

            notesTrack.Insert(absoluteTicks, MetaMessage.EndOfTrackMessage);
            metaTrack.Insert(absoluteTicks, MetaMessage.EndOfTrackMessage);
            return sequence;
        }

        internal void SaveToPDF(string fileName)
        {
            string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string tmpFileName = $"{fileName}-tmp.ly";
            SaveToLilypond(tmpFileName);

            string lilypondLocation = @"C:\Program Files (x86)\LilyPond\usr\bin\lilypond.exe";
            string sourceFolder = Path.GetDirectoryName(tmpFileName);
            string sourceFileName = Path.GetFileNameWithoutExtension(tmpFileName);
            string targetFolder = Path.GetDirectoryName(fileName);
            string targetFileName = Path.GetFileNameWithoutExtension(fileName);

            var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = sourceFolder,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("--pdf \"{0}\\{1}.ly\"", sourceFolder, sourceFileName),
                    FileName = lilypondLocation
                }
            };

            process.Start();
            while (!process.HasExited) { /* Wait for exit */
                }
                if (sourceFolder != targetFolder || sourceFileName != targetFileName)
            {
                File.Move(sourceFolder + "\\" + sourceFileName + ".pdf", targetFolder + "\\" + targetFileName + ".pdf");
                File.Delete(tmpFileName);
            }
        }

        internal void SaveToLilypond(string fileName)
        {
            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                outputFile.Write(LilypondViewModel.LilypondText);
                outputFile.Close();
            }
        }
        #endregion Saving to files
    }
}
