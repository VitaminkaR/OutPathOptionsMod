using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InfiniteEnergy", Category = "Builds", ID = 1)]
    public class InfiniteEnergyTweak : Tweak
    {
        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INFINITE ENERGY]");
            var toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
            Activate(toggle.Value);
            toggle.OnChangeValue += (bool v) => Activate(v);
        }

        private void Activate(bool v)
        {
            if (v)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var types = assembly.GetTypes().Where(t => t.Name.StartsWith("InfiniteEnergyPatches_"));
                foreach (var type in types)
                {
                    _harmony.PatchAll(type);
                }
            }
            else
            {
                _harmony.UnpatchSelf();
            }
        }

        [HarmonyPatch(typeof(Build_WearStation), "UseEnergy")]
        private class InfiniteEnergyPatches_WearStation
        {
            private static void Prefix(Build_WearStation __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_BreakerManual), "UseEnergy")]
        private class InfiniteEnergyPatches_BreakerManual
        {
            private static void Prefix(Build_BreakerManual __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllConverter), "UseEnergy")]
        private class InfiniteEnergyPatches_ChlorophyllConverter
        {
            private static void Prefix(Build_ChlorophyllConverter __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllExtractor), "UseEnergy")]
        private class InfiniteEnergyPatches_ChlorophyllExtractor
        {
            private static void Prefix(Build_ChlorophyllExtractor __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_OreExtractor), "Update_Extracting")]
        private class InfiniteEnergyPatches_OreExtractor_Update_Extracting
        {
            private static void Prefix(Build_OreExtractor __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SlayerManual), "UseEnergy")]
        private class InfiniteEnergyPatches_Slayer
        {
            private static void Prefix(Build_SlayerManual __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SoilMiner), "UseEnergy")]
        private class InfiniteEnergyPatches_SoilMiner
        {
            private static void Prefix(Build_SoilMiner __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SoilMinerV2), "UseEnergy")]
        private class InfiniteEnergyPatches_SoilMinerV2
        {
            private static void Prefix(Build_SoilMinerV2 __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_Trapper), "UseEnergy")]
        private class InfiniteEnergyPatches_Trapper
        {
            private static void Prefix(Build_Trapper __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_Vaporizer), "UseEnergy")]
        private class InfiniteEnergyPatches_Vaporizer
        {
            private static void Prefix(Build_Vaporizer __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump_Manual), "UseEnergy")]
        private class InfiniteEnergyPatches_WaterPump
        {
            private static void Prefix(Build_WaterPump_Manual __instance)
            {
                __instance.energy = __instance.maxEnergy;
            }
        }
    }
}