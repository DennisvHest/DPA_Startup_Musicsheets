using DPA_Musicsheets.Domain;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.IO.Lilypond
{
    public class LilypondSectionStartEvent : ISectionStartEvent
    {
        public Barline Barline { get; }

        public LilypondSectionStartEvent(bool inAlternative, LilypondToken currentToken, ref int alternativeRepeatNumber)
        {
            if (inAlternative && currentToken.PreviousToken.TokenKind != LilypondTokenKind.SectionEnd)
            {
                alternativeRepeatNumber++;
                Barline = new Barline { AlternateRepeatGroup = alternativeRepeatNumber };
            }
        }
    }
}
