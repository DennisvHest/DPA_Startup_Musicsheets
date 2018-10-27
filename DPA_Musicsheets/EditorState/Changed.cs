using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Models {

    public class Changed : EditorState {

        public override bool CanQuit => false;

        public Changed(MainViewModel context) : base(context) { }

        public override void TextChanged()
        {
            _context.CurrentState = new Rendering(_context);
        }

        public override void Save()
        {
            _context.CurrentState = new Saved(_context);
        }
    }
}
