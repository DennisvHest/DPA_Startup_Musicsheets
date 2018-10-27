using System.Collections.Generic;
using System.Windows.Input;

namespace DPA_Musicsheets.Hotkeys.KeyHandlers
{
    public class RegularKeyHandler : KeyHandler
    {
        private static readonly Dictionary<Key, string> _keys = new Dictionary<Key, string>
        {
            { Key.S, "S" },
            { Key.P, "P" },
            { Key.M, "M" },
            { Key.T, "T" },
            { Key.L, "L" },
            { Key.D3, "3" },
            { Key.D4, "4" },
            { Key.D8, "8" }
        };

        protected override string TryHandle(List<Key> keys)
        {
            foreach (Key key in keys)
            {
                if (_keys.ContainsKey(key))
                {
                    string keyString = _keys[key];
                    keys.Remove(key);

                    return keyString;
                }
            }

            return "";
        }
    }
}
