using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "AutoCollect", Category = "Builds", ID = 6)]
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
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_CollectionNet), "Update")]
        private static class AutoCollectPatches_CollectionNet
        {
            private static void Prefix(Build_CollectionNet __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItem();
            }
        }

        [HarmonyPatch(typeof(Build_Collector), "Update")]
        private static class AutoCollectPatches_Collector
        {
            private static void Prefix(Build_Collector __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_CollectorInv), "Update")]
        private static class AutoCollectPatches_CollectorInv
        {
            private static void Prefix(Build_CollectorInv __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_CrabTrap), "Update")]
        private static class AutoCollectPatches_CrabTrap
        {
            private static void Prefix(Build_CrabTrap __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_Incubator), "Update")]
        private static class AutoCollectPatches_Incubator
        {
            private static void Prefix(Build_Incubator __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_Market), "Update")]
        private static class AutoCollectPatches_Market
        {
            private static void Prefix(Build_Market __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_SoilMiner), "Update")]
        private static class AutoCollectPatches_SoilMiner
        {
            private static void Prefix(Build_SoilMiner __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_SoilMinerV2), "Update")]
        private static class AutoCollectPatches_SoilMinerV2
        {
            private static void Prefix(Build_SoilMinerV2 __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_Supplier), "Update")]
        private static class AutoCollectPatches_Supplier
        {
            private static void Prefix(Build_Supplier __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_Vaporizer), "Update")]
        private static class AutoCollectPatches_Vaporizer
        {
            private static void Prefix(Build_Vaporizer __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump), "Update")]
        private static class AutoCollectPatches_WaterPump
        {
            private static void Prefix(Build_WaterPump __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump_Manual), "Update")]
        private static class AutoCollectPatches_WaterPumpManual
        {
            private static void Prefix(Build_WaterPump_Manual __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItemsFromClicker();
            }
        }

        [HarmonyPatch(typeof(Build_ReceptionTower), "Update")]
        private static class AutoCollectPatches_ReceptionTower
        {
            private static void Prefix(Build_ReceptionTower __instance)
            {
                if (_toggle.Value)
                    __instance.TakeOutItems();
            }
        }
    }
}