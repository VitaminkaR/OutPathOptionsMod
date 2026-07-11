using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "AutoCollect", Category = "Builds")]
    public class AutoCollectTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[AUTO COLLECT]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(Build_BeeHive), "Update")]
        private static class AutoCollectPatches_BeeHive
        {
            private static void Prefix(Build_BeeHive __instance)
            {
                if (_toggle.Value && __instance.currCapacity > 0)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_CollectionNet), "Update")]
        private static class AutoCollectPatches_CollectionNet
        {
            private static void Prefix(Build_CollectionNet __instance)
            {
                if (_toggle.Value && __instance.canTakeItem)
                    __instance.TakeOutItem();
            }
        }

        [HarmonyPatch(typeof(Build_CrabTrap), "Update")]
        private static class AutoCollectPatches_CrabTrap
        {
            private static void Prefix(Build_CrabTrap __instance)
            {
                if (_toggle.Value && __instance.currCrabsCought > 0)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_Market), "Update")]
        private static class AutoCollectPatches_Market
        {
            private static void Prefix(Build_Market __instance)
            {
                if (_toggle.Value && __instance.currCredits > 1000)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_SoilMiner), "Update")]
        private static class AutoCollectPatches_SoilMiner
        {
            private static void Prefix(Build_SoilMiner __instance)
            {
                if (_toggle.Value && __instance.currCapacity > 0)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_SoilMinerV2), "Update")]
        private static class AutoCollectPatches_SoilMinerV2
        {
            private static void Prefix(Build_SoilMinerV2 __instance)
            {
                if (_toggle.Value && __instance.currCapacity > 0)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_Vaporizer), "Update")]
        private static class AutoCollectPatches_Vaporizer
        {
            private static void Prefix(Build_Vaporizer __instance)
            {
                if (_toggle.Value && __instance.currCapacity > 0)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump), "Update")]
        private static class AutoCollectPatches_WaterPump
        {
            private static void Prefix(Build_WaterPump __instance)
            {
                if (_toggle.Value && __instance.currCapacity > 0)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump_Manual), "Update")]
        private static class AutoCollectPatches_WaterPumpManual
        {
            private static void Prefix(Build_WaterPump_Manual __instance)
            {
                if (_toggle.Value && __instance.currCapacity > 0)
                    __instance.TakeOutItemsFromClicker();
            }
        }

        [HarmonyPatch(typeof(Build_ReceptionTower), "Update")]
        private static class AutoCollectPatches_ReceptionTower
        {
            private static void Prefix(Build_ReceptionTower __instance)
            {
                if (_toggle.Value && __instance.currQuantity > 0)
                    __instance.TakeOutItems();
            }
        }
    }
}