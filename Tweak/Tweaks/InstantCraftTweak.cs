using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantCraft", Category = "Builds", ID = 0)]
    public class InstantCraftTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INSTANT CRAFT]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(Build_Craft), "Update")]
        private static class InstantCraftPatches
        {
            private static void Prefix(Build_Craft __instance)
            {
                if (_toggle.Value)
                    __instance._timeToCraft = 0;
            }
        }
    }
}