using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantBuildBreak", Category = "Builds", ID = 2)]
    public class InstantBuildBreakTweak : Tweak
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
            {
                _harmony.PatchAll(typeof(InstantCraftPatches_SlayerManual));
                _harmony.PatchAll(typeof(InstantCraftPatches_WearStationVoid));
                _harmony.PatchAll(typeof(InstantCraftPatches_WearStation));
                _harmony.PatchAll(typeof(InstantCraftPatches_BreakerManual));
                _harmony.PatchAll(typeof(InstantCraftPatches_Breaker));
            }
            else
            {
                _harmony.UnpatchSelf();
            }
        }

        [HarmonyPatch(typeof(Build_SlayerManual), "Update_Searching")]
        private static class InstantCraftPatches_SlayerManual
        {
            private static void Prefix(Build_SlayerManual __instance)
            {
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_SlayerManual __instance)
            {
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }

        [HarmonyPatch(typeof(Build_WearStationVoid), "DamageResources")]
        private static class InstantCraftPatches_WearStationVoid
        {
            private static void Prefix(Build_WearStationVoid __instance)
            {
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_WearStationVoid __instance)
            {
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }

        [HarmonyPatch(typeof(Build_WearStation), "DamageResources")]
        private static class InstantCraftPatches_WearStation
        {
            private static void Prefix(Build_WearStation __instance)
            {
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_WearStation __instance)
            {
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }

        [HarmonyPatch(typeof(Build_BreakerManual), "Update_Searching")]
        private static class InstantCraftPatches_BreakerManual
        {
            private static void Prefix(Build_BreakerManual __instance)
            {
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_BreakerManual __instance)
            {
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }

        [HarmonyPatch(typeof(Build_Breaker), "Update_Searching")]
        private static class InstantCraftPatches_Breaker
        {
            private static void Prefix(Build_Breaker __instance)
            {
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_Breaker __instance)
            {
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }
    }
}