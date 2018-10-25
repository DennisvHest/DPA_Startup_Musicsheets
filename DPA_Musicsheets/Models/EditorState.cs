using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Models {

    public abstract class EditorState
    {
        protected readonly MainViewModel _context;

        public virtual bool CanQuit => false;
        public virtual string StateText => "";

        protected EditorState(MainViewModel context)
        {
            _context = context;
        }

        public virtual void TextChanged() { }
        public virtual void RenderingFinished() { }
        public virtual void Save() { }
    }
}
