using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;
using UnityEngine.UI;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantInteract", Category = "Player", ID = 1)]
    public class InstantInteractTweak : Tweak
    {
        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INSTANT INTERACT]");
            var toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
            Activate(toggle.Value);
            toggle.OnChangeValue += (bool v) => Activate(v);
        }

        private void Activate(bool v)
        {
            if (v)
                _harmony.PatchAll(typeof(InstantInteractPatches));
            else
                _harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(PlayerGarden), "Update")]
        private static class InstantInteractPatches
        {
            private static void Postfix(PlayerGarden __instance)
            {
                __instance.healthyStateIncreaseMult = 0;
            }
        }
    }
}