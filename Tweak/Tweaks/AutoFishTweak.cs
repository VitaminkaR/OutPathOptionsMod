using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "AutoFish", Category = "Player")]
    public class AutoFishTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[AUTO FISH]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(PlayerFishingManager), "Update")]
        private static class AutoFishPatches_PlayerFishingManager
        {
            private static void Prefix(PlayerFishingManager __instance)
            {
                if (_toggle.Value)
                {
                    __instance.currFishCatch = __instance.maxFishCatch;
                    __instance.currTresureCatch = __instance.maxTresureCatch;
                }
            }
        }

        [HarmonyPatch(typeof(FishingRodBait), "Update")]
        private static class AutoFishPatches_FishingRodBait
        {
            private static void Prefix(FishingRodBait __instance)
            {
                if (_toggle.Value && __instance.isInWater)
                {
                    PlayerFishingManager.instance.StartFishingMinigame();
                    __instance.canCatchFish = false;
                }
            }
        }
    }
}