using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;
using UnityEngine.UI;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantCraft", Category = "Builds", ID = 0)]
    public class InstantCraftTweak : Tweak
    {
        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INSTANT CRAFT]");
            var toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
            Activate(toggle.Value);
            toggle.OnChangeValue += (bool v) => Activate(v);
        }

        private void Activate(bool v)
        {
            if (v)
                _harmony.PatchAll(typeof(InstantCraftPatches));
            else
                _harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Build_Craft), "Update")]
        private static class InstantCraftPatches
        {
            private static void Prefix(Build_Craft __instance)
            {
                __instance._timeToCraft = 0;
            }
        }
    }
}