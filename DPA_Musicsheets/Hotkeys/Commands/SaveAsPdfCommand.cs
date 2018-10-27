using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Hotkeys.Commands
{
    public class SaveAsPdfCommand : ILilyEditorCommand
    {
        public string Pattern => "saveAsPdf";

        public void Execute(LilypondViewModel editor)
        {
            editor.SaveAsCommand.Execute("PDF|*.pdf");
        }
    }
}
