using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hotkeys
{
    public enum ModifierKey {
        NONE = 0x0000,
        ALT = 0x0001,
        CTRL = 0x0002,
        SHIFT = 0x0004,
        WIN = 0x0008
    }

    /// <summary>
    /// Class for registering a hotkey to a function. Does not work for console applications.
    /// </summary>
    public class HotkeyService : NativeWindow
    {

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private int id;
        private Action action;

        private int WM_HOTKEY = 0x312;

        /// <summary>
        /// Creates the window handle.
        /// </summary>
        public HotkeyService() {
            this.CreateHandle(new CreateParams());
        }

        /// <summary>
        /// Calls the registered function if the registered hotkey was pressed.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            if (m.Msg == WM_HOTKEY)
            {
                action();
            }
        }

        /// <summary>
        /// Registers a function to be called when a hotkey is pressed.
        /// </summary>
        /// <param name="hotkey">
        /// The hotkey to listen for.
        /// </param>
        /// <param name="action">
        /// The function to call.
        /// </param>
        public void Register(Hotkey hotkey, Action action)
        {
            UnregisterHotKey(Handle, id);
            int modifierKey = (int) hotkey.Modifier;
            int key = (int) hotkey.Key;
            id = modifierKey ^ key ^ Handle.ToInt32();
            RegisterHotKey(Handle, id, modifierKey, key);
            this.action = action;
        }
    }
}