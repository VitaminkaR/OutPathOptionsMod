using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "CreditsAdd", Category = "Player", ID = 3)]
    public class CreditsAddTweak : Tweak
    {
        private static KeyConfigurationElement _key;

        private static IntConfigurationElement _value;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[ADD CREDITS]");
            _key = KeyConfigurationElement.Create(GetConfigurations(), Name + "_toggle", "Toggle", UnityEngine.KeyCode.P);
            _value = IntConfigurationElement.Create(GetConfigurations(), Name + "_credits", "Credits Count", 100, 0, int.MaxValue);

            _harmony.PatchAll(typeof(CreditsAddPatches));
        }

        [HarmonyPatch(typeof(PlayerGarden), "Update")]
        private static class CreditsAddPatches
        {
            private static void Postfix(PlayerGarden __instance)
            {
                if (_key.Value.IsPressed())
                    __instance.AddCredits(_value.Value);
            }
        }
    }
}