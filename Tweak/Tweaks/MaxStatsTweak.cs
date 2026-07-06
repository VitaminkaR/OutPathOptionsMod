using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "MaxStats")]
    public class MaxStatsTweak : Tweak
    {
        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "Infinity Stats");
            var toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
            Activate(toggle.Value);
            toggle.OnChangeValue += (bool v) => Activate(v);
        }

        private void Activate(bool v)
        {
            if (v)
                _harmony.PatchAll(typeof(MaxStatsPatches));
            else
                _harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(PlayerGarden), "Update")]
        private static class MaxStatsPatches
        {
            private static void Postfix(PlayerGarden __instance)
            {
                __instance.AddStat_Food(float.MaxValue);
                __instance.AddStat_Health(float.MaxValue);
                __instance.AddStat_Stamina(float.MaxValue);
                __instance.AddStat_Energy(float.MaxValue);
            }
        }
    }
}