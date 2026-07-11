using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantBuildBreak", Category = "Builds")]
    public class InstantBuildBreakTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INSTANT BREAK]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(Build_SlayerManual), "Update_Searching")]
        private static class InstantCraftPatches_SlayerManual
        {
            private static void Prefix(Build_SlayerManual __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_SlayerManual __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }

        [HarmonyPatch(typeof(Build_WearStationVoid), "DamageResources")]
        private static class InstantCraftPatches_WearStationVoid
        {
            private static void Prefix(Build_WearStationVoid __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_WearStationVoid __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }

        [HarmonyPatch(typeof(Build_WearStation), "DamageResources")]
        private static class InstantCraftPatches_WearStation
        {
            private static void Prefix(Build_WearStation __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_WearStation __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }

        [HarmonyPatch(typeof(Build_BreakerManual), "Update_Searching")]
        private static class InstantCraftPatches_BreakerManual
        {
            private static void Prefix(Build_BreakerManual __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_BreakerManual __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }

        [HarmonyPatch(typeof(Build_Breaker), "Update_Searching")]
        private static class InstantCraftPatches_Breaker
        {
            private static void Prefix(Build_Breaker __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = int.MaxValue;
                __instance.maxDamage = int.MaxValue;
            }
            private static void Postfix(Build_Breaker __instance)
            {
                if (!_toggle.Value) return;
                __instance.minDamage = 10;
                __instance.maxDamage = 10;
            }
        }
    }
}