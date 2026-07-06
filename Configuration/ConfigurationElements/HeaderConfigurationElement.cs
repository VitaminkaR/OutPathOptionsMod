using System;
using System.Collections.Generic;
using UnityEngine;

namespace OutPathOptionsMod.Configuration.ConfigurationElements
{
    public class HeaderConfigurationElement : ConfigurationElement
    {
        private string _title;

        private HeaderConfigurationElement(
            List<ConfigurationElement> configurations, 
            string id,
            string title) : base(id)
        {
            configurations.Add(this);
            _title = title;
        }

        public static HeaderConfigurationElement Create(
            List<ConfigurationElement> configurations, 
            string id,
            string title)
        {
            HeaderConfigurationElement element = new HeaderConfigurationElement(configurations, id, title);
            return element;
        }

        public override void Draw()
        {
            GUILayout.Label(_title);
        }
    }
}
