using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DPA_Musicsheets.Models;
using PSAMControlLibrary;
using Barline = DPA_Musicsheets.Domain.Barline;
using MusicalSymbol = DPA_Musicsheets.Domain.MusicalSymbol;
using MusicalSymbolDuration = DPA_Musicsheets.Domain.MusicalSymbolDuration;
using Note = DPA_Musicsheets.Domain.Note;
using Rest = DPA_Musicsheets.Domain.Rest;
using TimeSignature = DPA_Musicsheets.Domain.TimeSignature;

namespace DPA_Musicsheets.IO.Lilypond
{
    public class LilypondSequenceReader : SequenceReader
    {
        private static List<Char> notesorder = new List<Char> { 'c', 'd', 'e', 'f', 'g', 'a', 'b' };

        public LilypondSequenceReader(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string line in File.ReadAllLines(fileName))
            {
                sb.AppendLine(line);
            }

            string lilypondText = sb.ToString();

            LinkedList<LilypondToken> tokens = GetTokensFromLilypond(lilypondText);

            List<MusicalSymbol> symbols = new List<MusicalSymbol>();

            Clef currentClef = null;
            int previousOctave = 4;
            char previousNote = 'c';
            bool inRepeat = false;
            bool inAlternative = false;
            int alternativeRepeatNumber = 0;

            LilypondToken currentToken = tokens.First();
            while (currentToken != null)
            {
                // TODO: There are a lot of switches based on LilypondTokenKind, can't those be eliminated en delegated?
                // HINT: Command, Decorator, Factory etc.

                // TODO: Repeats are somewhat weirdly done. Can we replace this with the COMPOSITE pattern?
                switch (currentToken.TokenKind)
                {
                    case LilypondTokenKind.Unknown:
                        break;
                    case LilypondTokenKind.Repeat:
                        inRepeat = true;
                        IRepeatEvent repeatEvent = new LilypondRepeatEvent();
                        symbols.Add(repeatEvent.Barline);
                        break;
                    case LilypondTokenKind.SectionEnd:
                        ISectionEndEvent sectionEndEvent = new LilypondSectionEndEvent(ref inRepeat, ref alternativeRepeatNumber, ref inAlternative, currentToken);
                        if (sectionEndEvent.Barline != null)
                            symbols.Add(sectionEndEvent.Barline);
                        break;
                    case LilypondTokenKind.SectionStart:
                        if (inAlternative && currentToken.PreviousToken.TokenKind != LilypondTokenKind.SectionEnd)
                        {
                            alternativeRepeatNumber++;
                            symbols.Add(new Barline() { AlternateRepeatGroup = alternativeRepeatNumber });
                        }
                        break;
                    case LilypondTokenKind.Alternative:
                        inAlternative = true;
                        inRepeat = false;
                        currentToken = currentToken.NextToken; // Skip the first bracket open.
                        break;
                    case LilypondTokenKind.Note:
                        // Tied
                        // TODO: A tie, like a dot and cross or mole are decorations on notes. Is the DECORATOR pattern of use here?
                        NoteTieType tie = NoteTieType.None;
                        if (currentToken.Value.StartsWith("~"))
                        {
                            tie = NoteTieType.Stop;
                            var lastNote = symbols.Last(s => s is Note) as Note;
                            if (lastNote != null) lastNote.TieType = NoteTieType.Start;
                            currentToken.Value = currentToken.Value.Substring(1);
                        }
                        // Length
                        int noteLength = Int32.Parse(Regex.Match(currentToken.Value, @"\d+").Value);
                        // Crosses and Moles
                        int alter = 0;
                        alter += Regex.Matches(currentToken.Value, "is").Count;
                        alter -= Regex.Matches(currentToken.Value, "es|as").Count;
                        // Octaves
                        int distanceWithPreviousNote = notesorder.IndexOf(currentToken.Value[0]) - notesorder.IndexOf(previousNote);
                        if (distanceWithPreviousNote > 3) // Shorter path possible the other way around
                        {
                            distanceWithPreviousNote -= 7; // The number of notes in an octave
                        }
                        else if (distanceWithPreviousNote < -3)
                        {
                            distanceWithPreviousNote += 7; // The number of notes in an octave
                        }

                        if (distanceWithPreviousNote + notesorder.IndexOf(previousNote) >= 7)
                        {
                            previousOctave++;
                        }
                        else if (distanceWithPreviousNote + notesorder.IndexOf(previousNote) < 0)
                        {
                            previousOctave--;
                        }

                        // Force up or down.
                        previousOctave += currentToken.Value.Count(c => c == '\'');
                        previousOctave -= currentToken.Value.Count(c => c == ',');

                        previousNote = currentToken.Value[0];

                        var note = new Note(currentToken.Value[0].ToString().ToUpper(), alter, previousOctave, (MusicalSymbolDuration)noteLength, NoteStemDirection.Up, tie, new List<NoteBeamType>() { NoteBeamType.Single });
                        note.NumberOfDots += currentToken.Value.Count(c => c.Equals('.'));

                        symbols.Add(note);
                        break;
                    case LilypondTokenKind.Rest:
                        var restLength = Int32.Parse(currentToken.Value[1].ToString());
                        symbols.Add(new Rest((MusicalSymbolDuration)restLength));
                        break;
                    case LilypondTokenKind.Bar:
                        symbols.Add(new Barline() { AlternateRepeatGroup = alternativeRepeatNumber });
                        break;
                    case LilypondTokenKind.Clef:
                        currentToken = currentToken.NextToken;
                        if (currentToken.Value == "treble")
                            currentClef = new Clef(ClefType.GClef, 2);
                        else if (currentToken.Value == "bass")
                            currentClef = new Clef(ClefType.FClef, 4);
                        else
                            throw new NotSupportedException($"Clef {currentToken.Value} is not supported.");

                        symbols.Add(currentClef);
                        break;
                    case LilypondTokenKind.Time:
                        currentToken = currentToken.NextToken;
                        var times = currentToken.Value.Split('/');
                        symbols.Add(new TimeSignature(TimeSignatureType.Numbers, UInt32.Parse(times[0]), UInt32.Parse(times[1])));
                        break;
                    case LilypondTokenKind.Tempo:
                        // Tempo not supported
                        break;
                    default:
                        break;
                }
                currentToken = currentToken.NextToken;
            }
        }

        private static LinkedList<LilypondToken> GetTokensFromLilypond(string content)
        {
            LinkedList<LilypondToken> tokens = new LinkedList<LilypondToken>();

            foreach (string s in content.Split(' ').Where(item => item.Length > 0))
            {
                LilypondToken token = new LilypondToken()
                {
                    Value = s
                };

                switch (s)
                {
                    case "\\relative": token.TokenKind = LilypondTokenKind.Staff; break;
                    case "\\clef": token.TokenKind = LilypondTokenKind.Clef; break;
                    case "\\time": token.TokenKind = LilypondTokenKind.Time; break;
                    case "\\tempo": token.TokenKind = LilypondTokenKind.Tempo; break;
                    case "\\repeat": token.TokenKind = LilypondTokenKind.Repeat; break;
                    case "\\alternative": token.TokenKind = LilypondTokenKind.Alternative; break;
                    case "{": token.TokenKind = LilypondTokenKind.SectionStart; break;
                    case "}": token.TokenKind = LilypondTokenKind.SectionEnd; break;
                    case "|": token.TokenKind = LilypondTokenKind.Bar; break;
                    default: token.TokenKind = LilypondTokenKind.Unknown; break;
                }

                if (token.TokenKind == LilypondTokenKind.Unknown && new Regex(@"[~]?[a-g][,'eis]*[0-9]+[.]*").IsMatch(s))
                {
                    token.TokenKind = LilypondTokenKind.Note;
                }
                else if (token.TokenKind == LilypondTokenKind.Unknown && new Regex(@"r.*?[0-9][.]*").IsMatch(s))
                {
                    token.TokenKind = LilypondTokenKind.Rest;
                }

                if (tokens.Last != null)
                {
                    tokens.Last.Value.NextToken = token;
                    token.PreviousToken = tokens.Last.Value;
                }

                tokens.AddLast(token);
            }

            return tokens;
        }
    }
}
