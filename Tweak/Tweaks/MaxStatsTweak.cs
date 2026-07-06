using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "Max Stats")]
    public class MaxStatsTweak : Tweak
    {
        private static BoolConfigurationElement _Toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), "max_stats_header", "Infinity Stats");
            _Toggle = BoolConfigurationElement.Create(GetConfigurations(), "max_stats", "Toggle", false);

            plugin.GetHarmony().PatchAll();
        }

        [HarmonyPatch(typeof(PlayerGarden), "Update")]
        private class MaxStatsPatches
        {
            private static void Postfix(PlayerGarden __instance)
            {
                if (_Toggle.Value)
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