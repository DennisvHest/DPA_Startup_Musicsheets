using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Hotkeys.Commands
{
    public class AddCleffCommand : ILilyEditorCommand
    {
        public string Pattern => "addClef";
        public void Execute(LilypondViewModel editor)
        {
            editor.LilypondText = editor.LilypondText.Insert(editor.CaretIndex, @"\clef treble");
        }
    }
}
