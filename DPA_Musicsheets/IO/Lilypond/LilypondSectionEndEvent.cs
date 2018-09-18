using DPA_Musicsheets.Domain;
using DPA_Musicsheets.Models;
using PSAMControlLibrary;
using Barline = DPA_Musicsheets.Domain.Barline;

namespace DPA_Musicsheets.IO.Lilypond
{
    public class LilypondSectionEndEvent : ISectionEndEvent
    {
        public Barline Barline { get; }

        public LilypondSectionEndEvent(ref bool inRepeat, ref int alternativeRepeatNumber, ref bool inAlternative, LilypondToken currentToken)
        {
            if (inRepeat && currentToken.NextToken?.TokenKind != LilypondTokenKind.Alternative)
            {
                inRepeat = false;
                Barline = new Barline() { RepeatType = RepeatType.Backward, AlternateRepeatGroup = alternativeRepeatNumber };
            }
            else if (inAlternative && alternativeRepeatNumber == 1)
            {
                alternativeRepeatNumber++;
                Barline = new Barline() { RepeatType = RepeatType.Backward, AlternateRepeatGroup = alternativeRepeatNumber };
            }
            else if (inAlternative && currentToken.NextToken.TokenKind == LilypondTokenKind.SectionEnd)
            {
                inAlternative = false;
                alternativeRepeatNumber = 0;
            }
        }
    }
}
