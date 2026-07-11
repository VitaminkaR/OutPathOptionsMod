using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InfiniteEffect", Category = "Player")]
    public class InfiniteEffectTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INFINITE EFFECTS]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(StatusTimer), "Update")]
        private static class InfiniteEffectPatches
        {
            private static void Prefix(StatusTimer __instance)
            {
                if (_toggle.Value)
                    __instance.timeToDestroy = __instance.itemInfo.duration;
            }
        }
    }
}