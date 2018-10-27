﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.ViewModels;

namespace DPA_Musicsheets.Hotkeys.Commands
{
    public interface IEditorCommand
    {
        string Pattern { get; }

        void Execute(MainViewModel editor);
    }
}