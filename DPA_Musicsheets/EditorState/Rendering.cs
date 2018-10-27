using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Models {

    public class Rendering : EditorState {

        public override string StateText => "Rendering...";

        public Rendering(MainViewModel context) : base(context) { }

        public override void RenderingFinished()
        {
            _context.CurrentState = new Changed(_context);
        }
    }
}
