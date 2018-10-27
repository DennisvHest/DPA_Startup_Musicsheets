using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Hotkeys.Commands
{
    public class AddTempoCommand : ILilyEditorCommand
    {
        public string Pattern => "addTempo";

        public void Execute(LilypondViewModel editor)
        {
            editor.LilypondText = editor.LilypondText.Insert(editor.CaretIndex, @"\tempo 4=120");
        }
    }
}
