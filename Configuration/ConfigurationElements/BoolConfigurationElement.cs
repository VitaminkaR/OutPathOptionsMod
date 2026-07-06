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

        public bool Value { get; private set; }

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
            Value = _configEntry.Value;
            OnChangeValue += (bool v) => _configEntry.Value = v;
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
                OnChangeValue.Invoke(Value);
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
