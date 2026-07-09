using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantBreak", Category = "Player", ID = 2)]
    public class InstantBreakTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INSTANT BREAK]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(TakeOutResource), "TryTakeOut_General")]
        private static class InstantBreakPatches
        {
            private static void Prefix(TakeOutResource __instance)
            {
                if (_toggle.Value)
                    __instance.currHealth = 0;
            }
        }
    }
}