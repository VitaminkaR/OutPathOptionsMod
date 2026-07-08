using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantInteract", Category = "Player", ID = 1)]
    public class InstantInteractTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INSTANT INTERACT]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(PlayerGarden), "Update")]
        private static class InstantInteractPatches
        {
            private static void Postfix(PlayerGarden __instance)
            {
                if (!_toggle.Value) return;
                __instance.healthyStateIncreaseMult = 0;
            }
        }
    }
}