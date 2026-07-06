using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using OutPathOptionsMod.Configuration;
using OutPathOptionsMod.Tweaks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//TODO:
// World:
// Spawnrate

namespace OutPathOptionsMod
{
    [BepInPlugin("com.vrcompany.outpath.optionsmod", "OutPath Options Mod", "0.1")]
    public class OutPathOptionsMod : BaseUnityPlugin
    {
        public PluginInfo PluginInfo { get; } = new PluginInfo();

        // Player
        public static ConfigEntry<bool> configInstantInteract;
        public static ConfigEntry<bool> configInstantPlayerBreak;
        public static ConfigEntry<KeyboardShortcut> configCreditsAddKey;
        public static ConfigEntry<KeyboardShortcut> configDupeItemInHand;
        public static ConfigEntry<KeyboardShortcut> configGetAllItems;
        public static ConfigEntry<int> configCreditsAddCount;
        // Build
        public static ConfigEntry<bool> configInfiniteBattery;
        public static ConfigEntry<bool> configInstantCraft;
        public static ConfigEntry<bool> configInstantBreak;
        public static ConfigEntry<float> configRadiusMult;
        public static ConfigEntry<float> configSpeedMult;

        static private Harmony harmony = new Harmony("com.vrcompany.outpath.optionsmod");
        public Harmony GetHarmony() => harmony;

        static public List<Tweak> Tweaks;
        public List<Tweak> GetTweaks() => Tweaks;

        static private bool isWorldLoaded = false;
        private PlayerGarden playerGarden;

        private ConfigurationHandler configurationHandler;
        public ConfigurationHandler ConfigurationHandler => configurationHandler;

        private void Awake()
        {
            Logger.LogInfo($"Plugin OPPtionsMod is loaded!");

            List<IConfigureObject> configureObjects = new List<IConfigureObject>();
            configurationHandler = ConfigurationHandler.Create(this, configureObjects, "TWEAKS MENU");

            Logger.LogInfo($"Loading Tweaks...");
            Tweaks = new List<Tweak>();
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<TweakAttribute>() != null)
            .ToList();
            foreach (var item in types)
            {
                var attr = item.GetCustomAttribute<TweakAttribute>();
                Logger.LogInfo($"\t{attr.Name} init...");
                try
                {
                    Tweak tweak = (Tweak)gameObject.AddComponent(item);
                    tweak.Init(this);
                    Tweaks.Add(tweak);
                    configureObjects.Add(tweak);
                }
                catch (Exception e)
                {
                    Logger.LogError($"\t{attr.Name} init error {e}!");
                }
                Logger.LogInfo($"\t{attr.Name} init successful!");
            }

            // Player
            configInstantInteract = ((BaseUnityPlugin)this).Config.Bind<bool>("Player", "Instant Interact", false, "Sets whether \"clicks\" will have a cooldown.");
            configInstantPlayerBreak = ((BaseUnityPlugin)this).Config.Bind<bool>("Player", "Instant Break", false, "Sets whether the player will instantly break items.");
            configCreditsAddKey = ((BaseUnityPlugin)this).Config.Bind<KeyboardShortcut>("Player", "Add Credits Hotkey", new KeyboardShortcut(UnityEngine.KeyCode.Z), "Sets a hotkey, when pressed, credits will be added.");
            configDupeItemInHand = ((BaseUnityPlugin)this).Config.Bind<KeyboardShortcut>("Player", "Dupe Item In Hand", new KeyboardShortcut(UnityEngine.KeyCode.X), "Sets a hotkey, when pressed, the number of items in your hands will increase. Add the item for the dup to your inventory and click on the button.");
            configGetAllItems = ((BaseUnityPlugin)this).Config.Bind<KeyboardShortcut>("Player", "Get All Items", new KeyboardShortcut(UnityEngine.KeyCode.C), "Sets a hotkey, when pressed, all items lying on the ground will be picked up.");
            configCreditsAddCount = ((BaseUnityPlugin)this).Config.Bind<int>("Player", "Count Credits", 100, $"Sets the number of credits to be added when the {configCreditsAddKey.Value.MainKey} is clicked.");
            // Build
            configInfiniteBattery = ((BaseUnityPlugin)this).Config.Bind<bool>("Builds", "Infinity Battery", false, "Sets whether buildings will work without a charge.");
            configInstantCraft = ((BaseUnityPlugin)this).Config.Bind<bool>("Builds", "Instant Craft", false, "Sets whether the crafting will be instant.");
            configInstantBreak = ((BaseUnityPlugin)this).Config.Bind<bool>("Builds", "Instant Break", false, "Sets whether mining buildings will instantly break down objects.");
            configRadiusMult = ((BaseUnityPlugin)this).Config.Bind<float>("Builds", "Radius Mutiplier", 0, "Sets the radius of buildings (0 - off) !Setting up requires restarting the world!.");
            configSpeedMult = ((BaseUnityPlugin)this).Config.Bind<float>("Builds", "Speed Mutiplier", 0, "Sets an increase in the speed of buildings by n times (0 - off) !Setting up requires restarting the world!.");
        }

        private void Update()
        {
            if (playerGarden == null)
            {
                playerGarden = FindObjectOfType<PlayerGarden>();
                if (isWorldLoaded)
                    isWorldLoaded = false;
            }
            else
            {
                if (!isWorldLoaded)
                {
                    isWorldLoaded = true;
                    WorldLoaded();
                }
            }
        }

        private void WorldLoaded()
        {
            StartCoroutine(SetBuildsMul());
        }

        [HarmonyPatch(typeof(PlayerGarden), "Update")]
        private class HarmonyPatch_PlayerGarden_Update
        {
            private static void Postfix(PlayerGarden __instance)
            {


                if (configCreditsAddKey.Value.IsPressed())
                    __instance.AddCredits(configCreditsAddCount.Value);

                if (configDupeItemInHand.Value.IsPressed())
                    InventoryManager.instance.AddItemToInv(InventoryManager.instance.selectedHotbarSlot.itemInfo, 1);

                if (configInstantInteract.Value)
                    __instance.healthyStateIncreaseMult = 0;

                if (configGetAllItems.Value.IsDown())
                {
                    Transform child = SaveDataGarden.instance.objectPoolerTrans.GetChild(2);
                    for (int i = 0; i < child.childCount; i++)
                    {
                        GameObject gameObject = child.GetChild(i).gameObject;
                        if (gameObject.activeSelf)
                        {
                            gameObject.GetComponent<ItemPrefab>().DirectCollect();
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(TakeOutResource), "TryTakeOut_General")]
        private class HarmonyPatch_TakeOutResource_TryTakeOut_General
        {
            private static void Prefix(TakeOutResource __instance)
            {
                if (configInstantPlayerBreak.Value)
                    __instance.currHealth = 0;
            }
        }

        private IEnumerator SetBuildsMul()
        {
            yield return new WaitForSeconds(1);

            if (configRadiusMult.Value != 0 || configSpeedMult.Value != 0 || configInstantBreak.Value || configInfiniteBattery.Value)
            {
                Assembly asm = Assembly.GetAssembly(typeof(GameManager));
                foreach (var type in asm.GetTypes())
                {
                    if (type.Name.StartsWith("Build_"))
                    {
                        if (configRadiusMult.Value != 0)
                        {
                            FieldInfo field = type.GetField("radius");
                            if (field != null)
                            {
                                foreach (var obj in FindObjectsOfType(type))
                                {
                                    field.SetValue(obj, (float)field.GetValue(obj) * configRadiusMult.Value);
                                }
                            }
                        }

                        if (configSpeedMult.Value != 0)
                        {
                            foreach (var field in type.GetFields())
                            {
                                if (field.Name.StartsWith("timeTo") || field.Name.StartsWith("timeOf") || field.Name.StartsWith("_timeTo"))
                                {
                                    foreach (var obj in FindObjectsOfType(type))
                                    {
                                        if (field.FieldType == typeof(Vector2))
                                            field.SetValue(obj, (Vector2)field.GetValue(obj) / configSpeedMult.Value);
                                        if (field.FieldType == typeof(float))
                                            field.SetValue(obj, (float)field.GetValue(obj) / configSpeedMult.Value);
                                    }
                                }
                            }
                        }

                        if (configInstantBreak.Value)
                        {
                            foreach (var field in type.GetFields())
                            {
                                if (field.Name.StartsWith("minDamage") || field.Name.StartsWith("maxDamage"))
                                {
                                    foreach (var obj in FindObjectsOfType(type))
                                    {
                                        field.SetValue(obj, 999999);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

        public GameObject GetGameObject() => gameObject;

        [HarmonyPatch(typeof(Build_Craft), "Update")]
        private class HarmonyPatch_Craft_Update
        {
            private static void Postfix(Build_Craft __instance)
            {
                if (configInstantCraft.Value)
                    __instance._timeToCraft = 0;
            }
        }

        #region INFINITY_ENERGY_PATCHES
        [HarmonyPatch(typeof(Build_WearStation), "UseEnergy")]
        private class HarmonyPatch_WearStation_UseEnergy
        {
            private static void Prefix(Build_WearStation __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_BreakerManual), "UseEnergy")]
        private class HarmonyPatch_BreakerManual_UseEnergy
        {
            private static void Prefix(Build_BreakerManual __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllConverter), "UseEnergy")]
        private class HarmonyPatch_ChlorophyllConverter_UseEnergy
        {
            private static void Prefix(Build_ChlorophyllConverter __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllExtractor), "UseEnergy")]
        private class HarmonyPatch_ChlorophyllExtractor_UseEnergy
        {
            private static void Prefix(Build_ChlorophyllExtractor __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_OreExtractor), "Update_Extracting")]
        private class HarmonyPatch_OreExtractor_Update_Extracting
        {
            private static void Prefix(Build_OreExtractor __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SlayerManual), "UseEnergy")]
        private class HarmonyPatch_Slayer_UseEnergy
        {
            private static void Prefix(Build_SlayerManual __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SoilMiner), "UseEnergy")]
        private class HarmonyPatch_SoilMiner_UseEnergy
        {
            private static void Prefix(Build_SoilMiner __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_SoilMinerV2), "UseEnergy")]
        private class HarmonyPatch_SoilMinerV2_UseEnergy
        {
            private static void Prefix(Build_SoilMinerV2 __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_Trapper), "UseEnergy")]
        private class HarmonyPatch_Trapper_UseEnergy
        {
            private static void Prefix(Build_Trapper __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_Vaporizer), "UseEnergy")]
        private class HarmonyPatch_Vaporizer_UseEnergy
        {
            private static void Prefix(Build_Vaporizer __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump_Manual), "UseEnergy")]
        private class HarmonyPatch_WaterPump_UseEnergy
        {
            private static void Prefix(Build_WaterPump_Manual __instance)
            {
                if (configInfiniteBattery.Value)
                    __instance.energy = __instance.maxEnergy;
            }
        }
        #endregion
    }
}
