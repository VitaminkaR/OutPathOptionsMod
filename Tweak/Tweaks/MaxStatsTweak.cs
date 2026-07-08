using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "MaxStats", Category = "Player", ID = 0)]
    public class MaxStatsTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INFINITE STATS]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(PlayerGarden), "Update")]
        private static class MaxStatsPatches
        {
            private static void Postfix(PlayerGarden __instance)
            {
                if (_toggle.Value)
                {
                    __instance.AddStat_Food(float.MaxValue);
                    __instance.AddStat_Health(float.MaxValue);
                    __instance.AddStat_Stamina(float.MaxValue);
                    __instance.AddStat_Energy(float.MaxValue);
                }
            }
        }
    }
}