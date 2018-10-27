using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Hotkeys.Commands
{
    public class SaveAsLilyCommand : ILilyEditorCommand
    {
        public string Pattern => "saveAsLily";

        public void Execute(LilypondViewModel editor)
        {
            editor.SaveAsCommand.Execute("Lilypond|*.ly");
        }
    }
}
