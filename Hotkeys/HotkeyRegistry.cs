using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hotkeys
{
    public enum ModifierKey
    {
        ALT = 0x0001,
        CTRL = 0x0002,
        SHIFT = 0x0004,
        WIN = 0x0008
    }

    /// <summary>
    /// Class for calling a function registered to a hotkey.
    /// </summary>
    public class HotkeyRegistry : NativeWindow
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_DOWN = 0x0312;
        private const int HOTKEY_DESTROY_ALL = 0x0002;
        private Dictionary<int, Action> dispatcher = new Dictionary<int, Action>();
        /// <summary>
        /// Creates window handle.
        /// </summary>
        public HotkeyRegistry()
        {
            CreateHandle(new CreateParams());
        }

        /// <summary>
        /// Calls the registered function if the registered hotkey is pressed.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (HOTKEY_DOWN):
                    int id = m.WParam.ToInt32();
                    dispatcher[id]();
                    break;
                case (HOTKEY_DESTROY_ALL):
                    foreach (KeyValuePair<int, Action> keyValuePair in dispatcher)
                    {
                        UnregisterHotKey(Handle, keyValuePair.Key);
                    }
                    dispatcher.Clear();
                    break;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Registers a function to be called when a hotkey is pressed.
        /// </summary>
        /// <param name="hotkey">
        /// The hotkey.
        /// </param>
        /// <param name="action">
        /// The function to be called.
        /// </param>
        public void Register(Hotkey hotkey, Action action)
        {
            //if (IsRegistered(hotkey))
            //{
            //    throw new ArgumentException($"Hotkey {hotkey.ModifierKey.ToString()} + {hotkey.Key.ToString()} is already registered.");
            //}
            Unregister(hotkey);
            int id = GetID(hotkey);
            RegisterHotKey(Handle, id, (int)hotkey.ModifierKey, (int)hotkey.Key);
            dispatcher[id] = action;
        }

        public bool IsRegistered(Hotkey hotkey)
        {
            int id = GetID(hotkey);
            return dispatcher.ContainsKey(id);
        }

        public void Unregister(Hotkey hotkey)
        {
            int id = GetID(hotkey);
            UnregisterHotKey(Handle, id);
            dispatcher.Remove(id);
        }

        public void UnregisterAll()
        {
            foreach (int id in dispatcher.Keys)
            {
                UnregisterHotKey(Handle, id);
            }
            dispatcher.Clear();
        }

        private int GetID(Hotkey hotkey)
        {
            int modifierPart = (int)hotkey.ModifierKey;
            int keyPart = (int)hotkey.Key;
            return modifierPart ^ keyPart ^ Handle.ToInt32();
        }
    }
}