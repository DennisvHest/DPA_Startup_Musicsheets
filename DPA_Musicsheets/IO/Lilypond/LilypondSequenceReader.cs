using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DPA_Musicsheets.Domain;
using DPA_Musicsheets.IO.Lilypond.Interpreter;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.IO.Lilypond
{
    public class LilypondSequenceReader : SequenceReader
    {
        public static List<char> NotesOrder = new List<char> { 'c', 'd', 'e', 'f', 'g', 'a', 'b' };

        public LilypondSequenceReader(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string line in File.ReadAllLines(fileName))
            {
                sb.AppendLine(line);
            }

            string[] lilypondText = sb.ToString().Split().Where(item => item.Length > 0).ToArray();

            LilypondContext context = new LilypondContext();
            Stack<LilypondSection> sections = new Stack<LilypondSection>();
            LilypondSection rootSection = null;

            for (int i = 0; i < lilypondText.Length; i++)
            {
                switch (lilypondText[i])
                {
                    case "\\relative":
                        // New relative section start with a startPitch and an ocatveChange
                        RelativeExpression relativeExpression = new RelativeExpression(lilypondText[i + 1][0],
                            string.Concat(lilypondText[i + 1].Skip(1)));

                        sections.Push(relativeExpression);

                        if (rootSection == null)
                            rootSection = relativeExpression;

                        i += 2;
                        break;
                    case "\\clef":
                        sections.Peek().ChildExpressions.Add(new ClefExpression(lilypondText[i + 1]));
                        i++;
                        break;
                    case "\\time":
                        sections.Peek().ChildExpressions.Add(new TimeSignatureExpression(lilypondText[i + 1]));
                        i++;
                        break;
                    // Section has ended. It is no longer the current section, so pop it from the stack
                    case "}": sections.Pop(); break;
                    // It is a note
                    default: sections.Peek().ChildExpressions.Add(new NoteExpression(lilypondText[i])); break;
                }
            }

            rootSection.Interpret(context);

            if (!context.ClefAdded)
                context.Sequence.Symbols.Add(new Clef(ClefType.GClef));

            Sequence = context.Sequence;
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
