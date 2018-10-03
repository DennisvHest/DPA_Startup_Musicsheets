namespace DPA_Musicsheets.Domain
{
    public class Mole : NoteDecoration
    {
        public override int NoteAlteration => Note.NoteAlteration - 1;
    }
}
