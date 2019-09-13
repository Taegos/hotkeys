using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotkeys {

    /// <summary>
    /// The hotkey structure.
    /// </summary>
    public class Hotkey {
        public ModifierKey ModifierKey { get; set; }
        public Keys Key { get; set; }

        public Hotkey(ModifierKey modifierKey, Keys key)
        {
            ModifierKey = modifierKey;
            Key = key;
        }
    }
}
