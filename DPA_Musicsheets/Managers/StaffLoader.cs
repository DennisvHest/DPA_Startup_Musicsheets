﻿using System.Collections.Generic;
using DPA_Musicsheets.Domain;

namespace DPA_Musicsheets.Managers
{
    public class StaffLoader
    {
        private readonly StaffBuilder _staffBuilder;

        public List<PSAMControlLibrary.MusicalSymbol> Symbols => _staffBuilder.Symbols;

        public StaffLoader(StaffBuilder staffBuilder)
        {
            _staffBuilder = staffBuilder;
        }

        public void LoadStaffs(IEnumerable<MusicalSymbol> symbols)
        {
            foreach (MusicalSymbol musicalSymbol in symbols)
            {
                switch (musicalSymbol)
                {
                    case TimeSignature timeSignature:
                        _staffBuilder.AddSymbol(timeSignature);
                        break;
                    case Barline barLine:
                        _staffBuilder.AddSymbol(barLine);
                        break;
                    case Note note:
                        _staffBuilder.AddSymbol(note);
                        break;
                }
            }
        }
    }
}