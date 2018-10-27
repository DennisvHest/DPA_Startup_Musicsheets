using System.Collections.Generic;
using System.Windows.Input;

namespace DPA_Musicsheets.Hotkeys.KeyHandlers
{
    public class CommandKeyHandler : KeyHandler
    {
        private static readonly Dictionary<Key, string> _commandKeys = new Dictionary<Key, string>
        {
            { Key.LeftCtrl, "CTRL" }
        };

        protected override string TryHandle(List<Key> keys)
        {
            foreach (Key key in keys)
            {
                if (_commandKeys.ContainsKey(key))
                {
                    string keyString = _commandKeys[key];
                    keys.Remove(key);

                    return keyString;
                }
            }

            return "";
        }
    }
}
