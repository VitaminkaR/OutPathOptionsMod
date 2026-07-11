using BepInEx.Logging;
using OutPathOptionsMod.Configuration;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    public class Tweak : MonoBehaviour, IConfigureObject
    {
        protected List<ConfigurationElement> _configurations;

        public List<ConfigurationElement> GetConfigurations() => _configurations;

        public string Name => GetType().GetCustomAttribute<TweakAttribute>().Name;

        public ManualLogSource Logger { get; private set; }

        public virtual void Init(OutPathOptionsMod plugin)
        {
            _configurations = new List<ConfigurationElement>();
            Logger = OutPathOptionsMod.GetLogger;
        }

        string IConfigureObject.GetCategory() => GetType().GetCustomAttribute<TweakAttribute>().Category;
    }
}
