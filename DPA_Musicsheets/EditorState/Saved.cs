using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Models {

    public class Saved : EditorState {

        public override bool CanQuit => true;

        public Saved(MainViewModel context) : base(context) { }

        public override void TextChanged()
        {
            _context.CurrentState = new Rendering(_context);
        }
    }
}
