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
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INFINITE ENERGY]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(Build_WearStation), "UseEnergy")]
        private class InfiniteEnergyPatches_WearStation
        {
            private static void Prefix(Build_WearStation __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_BreakerManual), "UseEnergy")]
        private class InfiniteEnergyPatches_BreakerManual
        {
            private static void Prefix(Build_BreakerManual __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllConverter), "UseEnergy")]
        private class InfiniteEnergyPatches_ChlorophyllConverter
        {
            private static void Prefix(Build_ChlorophyllConverter __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllExtractor), "UseEnergy")]
        private class InfiniteEnergyPatches_ChlorophyllExtractor
        {
            private static void Prefix(Build_ChlorophyllExtractor __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_OreExtractor), "Update_Extracting")]
        private class InfiniteEnergyPatches_OreExtractor
        {
            private static void Prefix(Build_OreExtractor __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SlayerManual), "UseEnergy")]
        private class InfiniteEnergyPatches_Slayer
        {
            private static void Prefix(Build_SlayerManual __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SoilMiner), "UseEnergy")]
        private class InfiniteEnergyPatches_SoilMiner
        {
            private static void Prefix(Build_SoilMiner __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SoilMinerV2), "UseEnergy")]
        private class InfiniteEnergyPatches_SoilMinerV2
        {
            private static void Prefix(Build_SoilMinerV2 __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_Trapper), "UseEnergy")]
        private class InfiniteEnergyPatches_Trapper
        {
            private static void Prefix(Build_Trapper __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_Vaporizer), "UseEnergy")]
        private class InfiniteEnergyPatches_Vaporizer
        {
            private static void Prefix(Build_Vaporizer __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump_Manual), "UseEnergy")]
        private class InfiniteEnergyPatches_WaterPump
        {
            private static void Prefix(Build_WaterPump_Manual __instance)
            {
                if (_toggle.Value) __instance.energy = __instance.maxEnergy;
            }
        }
    }
}