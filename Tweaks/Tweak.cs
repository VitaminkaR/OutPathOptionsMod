using BepInEx;
using OutPathOptionsMod.Configuration;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using System.Collections.Generic;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    public class Tweak : MonoBehaviour, IConfigureObject
    {
        protected List<ConfigurationElement> _configurations;

        public List<ConfigurationElement> GetConfigurations() => _configurations;

        public virtual void Init(OutPathOptionsMod plugin)
        {
            _configurations = new List<ConfigurationElement>();
        }
    }
}
