using System;
using System.Collections.Generic;
using UnityEngine;

namespace OutPathOptionsMod.Configuration.ConfigurationElements
{
    public class ButtonConfigurationElement : ConfigurationElement
    {
        private string _title;

        public event Action OnChangeValue;

        private ButtonConfigurationElement(
            List<ConfigurationElement> configurations,
            string id,
            string title) : base(id)
        {
            configurations.Add(this);
            _title = title;
        }

        public static ButtonConfigurationElement Create(
            List<ConfigurationElement> configurations,
            string id,
            string title)
        {
            ButtonConfigurationElement element = new ButtonConfigurationElement(configurations, id, title);
            return element;
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_title)) OnChangeValue.Invoke();
            GUILayout.EndHorizontal();
        }
    }
}
