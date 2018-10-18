namespace DPA_Musicsheets.Models {

    public class LilypondOriginator {

        public string Text { get; set; }

        public LilypondMemento Save()
        {
            return new LilypondMemento(Text);
        }

        public void Restore(LilypondMemento memento)
        {
            Text = memento.Text;
        }
    }
}
