using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using OutPathOptionsMod.Tweaks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace OutPathOptionsMod.Configuration
{
    public class ConfigurationHandler : MonoBehaviour
    {
        public const float ELEMENT_SPACE_DIVISION_SIZE = 16;

        private CursorLockMode _cursorState;

        private bool _cursorVisible;

        private bool _menuVisible = false;

        private Dictionary<string, List<IConfigureObject>> _configureObjects;

        private bool _isChangeKey;

        private Rect _windowRect = new Rect(100, 100, 720, 680);

        private Vector2 _scrollPosition;

        private string _keyString;

        public string MenuHeader { get; set; }

        public KeyCode OpenMenuKey { get; private set; }

        public static ConfigFile Config;

        private ConfigEntry<KeyboardShortcut> _openMenuKeyEntry;

        private event Action<KeyCode> _onChangeOpenMenuKey;

        static public ConfigurationHandler Create(
            BaseUnityPlugin plugin,
            string MenuHeader = "Configuration Menu",
            KeyCode openMenuKey = KeyCode.F1)
        {
            ConfigurationHandler handler = plugin.gameObject.AddComponent<ConfigurationHandler>();

            handler._configureObjects = new Dictionary<string, List<IConfigureObject>>();

            handler.MenuHeader = MenuHeader;

            handler.OpenMenuKey = openMenuKey;

            Config = new ConfigFile(Path.Combine(Paths.ConfigPath, $"{plugin.GetType().Name}_configuration.cfg"), true);

            handler._openMenuKeyEntry = Config.Bind<KeyboardShortcut>("Main", "open_menu", new KeyboardShortcut(openMenuKey));
            handler.OpenMenuKey = handler._openMenuKeyEntry.Value.MainKey;
            handler._onChangeOpenMenuKey += (KeyCode kc) => handler._openMenuKeyEntry.Value = new KeyboardShortcut(kc);

            return handler;
        }

        public void AddConfigureCategory(string category)
        {
            _configureObjects.Add(category, new List<IConfigureObject>());
        }

        public void AddConfigureObject(IConfigureObject obj)
        {
            string cat = obj.GetCategory();

            if (string.IsNullOrEmpty(cat)) cat = "Main";

            if (!_configureObjects.ContainsKey(cat))
                _configureObjects.Add(obj.GetCategory(), new List<IConfigureObject>());

            _configureObjects[cat].Add(obj);

            _configureObjects[cat].Sort((x, y) => x.GetID().CompareTo(y.GetID()));
        }

        private void OpenMenu()
        {
            _cursorState = Cursor.lockState;
            _cursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _menuVisible = true;
        }

        private void CloseMenu()
        {
            Cursor.lockState = _cursorState;
            Cursor.visible = _cursorVisible;
            _menuVisible = false;
        }

        private void OnGUI()
        {
            _keyString = OpenMenuKey.ToString();

            if (!_isChangeKey && Event.current.Equals(Event.KeyboardEvent(_keyString)))
            {
                if (_menuVisible) CloseMenu(); else OpenMenu();
            }

            if (_isChangeKey)
            {
                _keyString = "Wait";
                if (Event.current.isKey)
                {
                    OpenMenuKey = Event.current.keyCode;
                    _isChangeKey = false;
                    _onChangeOpenMenuKey.Invoke(OpenMenuKey);
                }
            }

            if (_menuVisible)
            {
                GUI.skin.window.normal.textColor = Color.white;
                GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.99f);
                _windowRect = GUILayout.Window(570, _windowRect, DrawMenuWindow, MenuHeader, GUI.skin.window);
            }
        }

        void DrawMenuWindow(int windowID)
        {
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                normal = { textColor = Color.white }
            };

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close")) CloseMenu();
            if (GUILayout.Button($"Change key: [{_keyString}]")) _isChangeKey = true;
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            foreach (var key in _configureObjects.Keys)
            {
                GUILayout.Label($"=========[ {key.ToUpper()} ]=========", centeredStyle);

                foreach (var co in _configureObjects[key])
                {
                    foreach (var configuration in co.GetConfigurations())
                    {
                        if (!configuration.IsEnabled) continue;
                        configuration.Draw();
                    }
                    GUILayout.Space(ELEMENT_SPACE_DIVISION_SIZE);
                }
            }

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
    }
}
