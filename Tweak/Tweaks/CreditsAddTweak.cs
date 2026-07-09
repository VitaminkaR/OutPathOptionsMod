using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;
using UnityEngine.UI;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "CreditsAdd", Category = "Player", ID = 3)]
    public class CreditsAddTweak : Tweak
    {
        private KeyConfigurationElement _key;

        private IntConfigurationElement _value;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[ADD CREDITS]");
            _key = KeyConfigurationElement.Create(GetConfigurations(), Name + "_toggle", "Add Key", KeyCode.P);
            _value = IntConfigurationElement.Create(GetConfigurations(), Name + "_credits", "Count", 100, 0, int.MaxValue);
        }
        private void Update()
        {
            var player = PlayerGarden.instance;
            if (player != null && Input.GetKeyDown(_key.Value))
            {
                player.AddCredits(_value.Value);
            }
        }
    }
}