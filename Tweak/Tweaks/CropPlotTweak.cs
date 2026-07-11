using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "CropPlot", Category = "Builds")]
    public class CropPlotTweak : Tweak
    {
        private static BoolConfigurationElement _toggleInfiniteWater;
        private static BoolConfigurationElement _toggleInfiniteFert;
        private static BoolConfigurationElement _toggleFastGrow;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[CROP PLOT]");
            _toggleInfiniteWater = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_infinite_water", "Infinite Water", false);
            _toggleInfiniteFert = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_infinite_fert", "Infinite Fertilizer", false);
            _toggleFastGrow = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}__fast_grow", "Fast Grow", false);
        }

        [HarmonyPatch(typeof(Build_CropPlot), "AddTicks")]
        private static class CropPlotPatches_CropPlot
        {
            private static void Prefix(Build_CropPlot __instance)
            {
                if (_toggleInfiniteWater.Value)
                    __instance.waterLevel = 30;

                if (_toggleInfiniteFert.Value && __instance.fertilizerItemInfo != null)
                    __instance._fertilizerDuration = __instance.fertilizerItemInfo.cropFertilizerDuration;

                if (_toggleFastGrow.Value)
                {
                    var crops = __instance.crops;
                    for (int i = 0; i < crops.Length; i++)
                    {
                        var crop = crops[i];
                        crop.currTicks = crop.maxTicks;
                    }
                }
            }
        }
    }
}