using System.Text;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.IO.Lilypond.Writer {

    public class LilypondSequenceWriter
    {

        private readonly StringBuilder lilyBuilder;

        public LilypondSequenceWriter(MusicalSequence sequence)
        {
            lilyBuilder = new StringBuilder();

            lilyBuilder.Append("\\relative c {");
            
            lilyBuilder.Append("}");
        }
    }
}
