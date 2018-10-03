namespace DPA_Musicsheets.Domain
{
    public class Dot : NoteDecoration
    {
        public override int Dots => Note.Dots + 1;
    }
}
