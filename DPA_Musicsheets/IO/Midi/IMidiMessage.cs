using DPA_Musicsheets.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.IO.Midi
{
    public interface IMidiMessage
    {
        void Parse();
    }
}
