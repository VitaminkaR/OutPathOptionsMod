using BepInEx;
using BepInEx.Configuration;
using OutPathOptionsMod.Tweaks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace OutPathOptionsMod.Configuration
{
    public class ConfigurationHandler : MonoBehaviour
    {
        public const float ELEMENT_SPACE_DIVISION_SIZE = 16;

        private CursorLockMode _CursorState;

        private bool _CursorVisible;

        private bool _MenuVisible = false;

        private List<IConfigureObject> _ConfigureObjects;

        private bool _IsChangeKey;

        private Rect _WindowRect = new Rect(100, 100, 720, 680);

        private Vector2 scrollPosition;

        private string _KeyString;

        public string MenuHeader { get; set; }

        public KeyCode OpenMenuKey { get; private set; }

        public static ConfigFile Config;

        private ConfigEntry<KeyboardShortcut> _openMenuKeyEntry;

        private event Action<KeyCode> _onChangeOpenMenuKey;

        static public ConfigurationHandler Create(
            BaseUnityPlugin plugin,
            List<IConfigureObject> configureObjects = null,
            string MenuHeader = "Configuration Menu",
            KeyCode openMenuKey = KeyCode.F1)
        {
            ConfigurationHandler handler = plugin.gameObject.AddComponent<ConfigurationHandler>();

            handler._ConfigureObjects = configureObjects != null ? configureObjects : new List<IConfigureObject>();

            handler.MenuHeader = MenuHeader;

            handler.OpenMenuKey = openMenuKey;
            
            Config = new ConfigFile(Path.Combine(Paths.ConfigPath, $"{plugin.GetType().Name}_configuration.cfg"), true);

            handler._openMenuKeyEntry = Config.Bind<KeyboardShortcut>("Main", "open_menu", new KeyboardShortcut(openMenuKey));
            handler.OpenMenuKey = handler._openMenuKeyEntry.Value.MainKey;
            handler._onChangeOpenMenuKey += (KeyCode kc) => handler._openMenuKeyEntry.Value = new KeyboardShortcut(kc);

            return handler;
        }

        public void SetConfigureObjects(List<IConfigureObject> objects)
        {
            _ConfigureObjects = objects;
        }

        private void OpenMenu()
        {
            _CursorState = Cursor.lockState;
            _CursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _MenuVisible = true;
        }

        private void CloseMenu()
        {
            Cursor.lockState = _CursorState;
            Cursor.visible = _CursorVisible;
            _MenuVisible = false;
        }

        private void OnGUI()
        {
            _KeyString = OpenMenuKey.ToString();

            if (!_IsChangeKey && Event.current.Equals(Event.KeyboardEvent(_KeyString)))
            {
                if (_MenuVisible) CloseMenu(); else OpenMenu();
            }

            if (_IsChangeKey)
            {
                _KeyString = "Wait";
                if (Event.current.isKey)
                {
                    OpenMenuKey = Event.current.keyCode;
                    _IsChangeKey = false;
                    _onChangeOpenMenuKey.Invoke(OpenMenuKey);
                }
            }

            if (_MenuVisible)
            {
                GUI.skin.window.normal.textColor = Color.white;
                GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.99f);
                _WindowRect = GUILayout.Window(570, _WindowRect, DrawMenuWindow, MenuHeader, GUI.skin.window);
            }
        }

        void DrawMenuWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close")) CloseMenu();
            if (GUILayout.Button($"Change key: [{_KeyString}]")) _IsChangeKey = true;
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (var tweak in _ConfigureObjects)
            {
                foreach (var configuration in tweak.GetConfigurations())
                {
                    if (!configuration.IsEnabled) continue;
                    configuration.Draw();   
                }
                GUILayout.Space(ELEMENT_SPACE_DIVISION_SIZE);
            }
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
    }
}
