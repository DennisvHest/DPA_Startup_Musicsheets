using System;
using System.Collections.Generic;
using System.Linq;
using DPA_Musicsheets.Domain;
using DPA_Musicsheets.IO.Lilypond.Interpreter;

namespace DPA_Musicsheets.IO.Lilypond
{
    public class LilypondSequenceReader : SequenceReader
    {
        public LilypondSequenceReader(string lilyContent)
        {
            string[] lilypondText = lilyContent.Split().Where(item => item.Length > 0).ToArray();

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
                    case "\\repeat":
                        RepeatExpression repeatExpression = new RepeatExpression();

                        sections.Peek()?.ChildExpressions.Add(repeatExpression);
                        sections.Push(repeatExpression);

                        i += 3;
                        break;
                    case "\\alternative":
                        AlternativeExpression alternativeExpression = new AlternativeExpression();

                        sections.Peek()?.ChildExpressions.Add(alternativeExpression);
                        sections.Push(alternativeExpression);

                        context.InAlternative = true;
                        i++;
                        break;
                    case "\\clef":
                        sections.Peek().ChildExpressions.Add(new ClefExpression(lilypondText[i + 1]));
                        i++;
                        break;
                    case "\\tempo":
                        sections.Peek().ChildExpressions.Add(new TempoExpression(lilypondText[i + 1]));
                        i += 1;
                        break;
                    case "\\time":
                        sections.Peek().ChildExpressions.Add(new TimeSignatureExpression(lilypondText[i + 1]));
                        i++;
                        break;
                    case "{":
                        if (context.InAlternative)
                        {
                            // There is a new alternative group in the current alternative block
                            AlternativeGroupExpression alternativeGroup = new AlternativeGroupExpression();

                            sections.Peek()?.ChildExpressions.Add(alternativeGroup);
                            sections.Push(alternativeGroup);

                            context.InAlternativeGroup = true;
                        }
                        else
                        {
                            LilypondSection lilypondSection = new LilypondSection();

                            if (sections.Any())
                            {
                                sections.Peek()?.ChildExpressions.Add(lilypondSection);
                                sections.Push(lilypondSection);
                            }
                        }
                        break;
                    case "}": // Section has ended. It is no longer the current section, so pop it from the stack
                        if (context.InRepeat)
                        {
                            if (lilypondText[i + 1] != "\\alternative")
                            {
                                sections.Peek().ChildExpressions.Add(new BarlineExpression(true));
                            }

                            context.InRepeat = false;
                        }
                        if (sections.Any()) sections.Pop();
                        break;
                    case "|": sections.Peek().ChildExpressions.Add(new BarlineExpression()); break;
                    // It is a note or an unknown token
                    default:
                        try
                        {
                            sections.Peek().ChildExpressions.Add(new NoteExpression(lilypondText[i]));
                            break;
                        }
                        catch (Exception)
                        {
                            /* It is an unknown token, skip it. */ break;
                        }
                }
            }

            rootSection.Interpret(context);

            if (!context.ClefAdded)
                context.Sequence.Symbols.Insert(0, new Clef(ClefType.GClef));

            Sequence = context.Sequence;
        }
    }
}
