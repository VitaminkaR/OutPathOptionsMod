using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OutPathOptionsMod.Configuration.ConfigurationElements
{
    public class BoolConfigurationElement : ConfigurationElement
    {
        private string _title;

        private bool _default;

        public bool Value
        {
            get
            {
                if (_configEntry != null)
                {
                    return _configEntry.Value;
                }
                else
                {
                    return _default;
                }
            }

            private set
            {
                _configEntry.Value = value;
                OnChangeValue(value);
            }
        }

        public event Action<bool> OnChangeValue;

        private ConfigEntry<bool> _configEntry;

        private BoolConfigurationElement(
            List<ConfigurationElement> configurations,
            string id,
            string title,
            bool defvalue) : base(id)
        {
            configurations.Add(this);
            _title = title;
            _default = defvalue;
            _configEntry = _config.Bind<bool>("Config", id, defvalue);
        }

        public static BoolConfigurationElement Create(
            List<ConfigurationElement> configurations,
            string id,
            string title,
            bool defvalue)
        {
            BoolConfigurationElement element = new BoolConfigurationElement(configurations, id, title, defvalue);
            return element;
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            bool value = GUILayout.Toggle(Value, _title, GUI.skin.toggle);
            if (value != Value)
            {
                Value = value;
            }

            if (GUILayout.Button("Default", GUILayout.MinWidth(DEFAULT_BUTTON_SIZE), GUILayout.MaxWidth(DEFAULT_BUTTON_SIZE)))
            {
                Value = _default;
            }
            GUILayout.EndHorizontal();
        }
    }
}
