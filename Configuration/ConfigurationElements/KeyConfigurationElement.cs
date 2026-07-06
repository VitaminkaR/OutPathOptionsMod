using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OutPathOptionsMod.Configuration.ConfigurationElements
{
    public class KeyConfigurationElement : ConfigurationElement
    {
        private string _title;

        private KeyCode _default;

        public KeyCode Value { get; private set; }

        public event Action<KeyCode> OnChangeValue;

        private ConfigEntry<KeyboardShortcut> _configEntry;

        private bool _IsChangeKey;

        private string _KeyString;

        private KeyConfigurationElement(
            List<ConfigurationElement> configurations, 
            string id,
            string title, 
            KeyCode defvalue) : base(id)
        {
            configurations.Add(this);
            _title = title;
            _default = defvalue;
            _configEntry = _config.Bind<KeyboardShortcut>("Config", id, new KeyboardShortcut(defvalue));
            Value = _configEntry.Value.MainKey;
            OnChangeValue += (KeyCode v) => _configEntry.Value = new KeyboardShortcut(v);
        }

        public static KeyConfigurationElement Create(
            List<ConfigurationElement> configurations,
            string id,
            string title,
            KeyCode defvalue)
        {
            KeyConfigurationElement element = new KeyConfigurationElement(configurations, id, title, defvalue);
            return element;
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{_title}");

            if (GUILayout.Button($"Change key: [{_KeyString}]")) _IsChangeKey = true;

            if (_IsChangeKey)
            {
                _KeyString = "Wait";
                if (Event.current.isKey)
                {
                    Value = Event.current.keyCode;
                    _IsChangeKey = false;
                    OnChangeValue.Invoke(Value);
                }
            }
            else
            {
                _KeyString = Value.ToString();
            }

            if (GUILayout.Button("Default", GUILayout.MinWidth(DEFAULT_BUTTON_SIZE), GUILayout.MaxWidth(DEFAULT_BUTTON_SIZE)))
            {
                Value = _default;
                OnChangeValue.Invoke(Value);
            }
            GUILayout.EndHorizontal();
        }
    }
}
