using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Hotkeys.Commands
{
    public class AddTimeCommand : ILilyEditorCommand
    {
        private readonly uint _beatsPerBar;
        private readonly uint _beatUnit;

        public AddTimeCommand(uint beatUnit, uint beatsPerBar)
        {
            _beatUnit = beatUnit;
            _beatsPerBar = beatsPerBar;
        }

        public string Pattern => $"add{_beatUnit}{_beatsPerBar}Time";

        public void Execute(LilypondViewModel editor)
        {
            editor.LilypondText = editor.LilypondText.Insert(editor.CaretIndex, $"\\time {_beatUnit}/{_beatsPerBar}");
        }
    }
}
