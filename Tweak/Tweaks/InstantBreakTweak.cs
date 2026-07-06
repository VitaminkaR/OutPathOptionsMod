using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantBreak")]
    public class InstantBreakTweak : Tweak
    {
        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INSTANT BREAK]");
            var toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
            Activate(toggle.Value);
            toggle.OnChangeValue += (bool v) => Activate(v);
        }

        private void Activate(bool v)
        {
            if (v)
                _harmony.PatchAll(typeof(InstantBreakPatches));
            else
                _harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(TakeOutResource), "TryTakeOut_General")]
        private static class InstantBreakPatches
        {
            private static void Prefix(TakeOutResource __instance)
            {
                __instance.currHealth = 0;
            }
        }
    }
}