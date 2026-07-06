using HarmonyLib;
using OutPathOptionsMod.Configuration;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using System.Collections.Generic;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    public class Tweak : MonoBehaviour, IConfigureObject
    {
        protected Harmony _harmony;

        protected List<ConfigurationElement> _configurations;

        public List<ConfigurationElement> GetConfigurations() => _configurations;

        public virtual void Init(OutPathOptionsMod plugin)
        {
            _harmony = plugin.GetHarmony();
            _configurations = new List<ConfigurationElement>();
        }
    }
}
