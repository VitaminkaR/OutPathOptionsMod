using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
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

        // Build
        public static ConfigEntry<bool> configInfiniteBattery;
        public static ConfigEntry<bool> configInstantBreak;
        public static ConfigEntry<float> configRadiusMult;
        public static ConfigEntry<float> configSpeedMult;

        static private Harmony harmony = new Harmony("com.vrcompany.outpath.optionsmod");
        public Harmony GetHarmony() => harmony;

        static public List<Tweak> Tweaks;
        public List<Tweak> GetTweaks() => Tweaks;

        static private bool isWorldLoaded = false;

        private ConfigurationHandler configurationHandler;
        public ConfigurationHandler ConfigurationHandler => configurationHandler;

        private static ManualLogSource logger;

        public static ManualLogSource GetLogger => logger;

        private void Awake()
        {
            logger = Logger;

            Logger.LogInfo($"Plugin OPPtionsMod is loaded!");

            configurationHandler = ConfigurationHandler.Create(this, "TWEAKS MENU");

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
                    configurationHandler.AddConfigureObject(tweak);
                    Logger.LogInfo($"\t{attr.Name} init successful!");
                }
                catch (Exception e)
                {
                    Logger.LogError($"\t{attr.Name} init error {e}!");
                }
            }

            // Build
            configInfiniteBattery = ((BaseUnityPlugin)this).Config.Bind<bool>("Builds", "Infinity Battery", false, "Sets whether buildings will work without a charge.");
            configInstantBreak = ((BaseUnityPlugin)this).Config.Bind<bool>("Builds", "Instant Break", false, "Sets whether mining buildings will instantly break down objects.");
            configRadiusMult = ((BaseUnityPlugin)this).Config.Bind<float>("Builds", "Radius Mutiplier", 0, "Sets the radius of buildings (0 - off) !Setting up requires restarting the world!.");
            configSpeedMult = ((BaseUnityPlugin)this).Config.Bind<float>("Builds", "Speed Mutiplier", 0, "Sets an increase in the speed of buildings by n times (0 - off) !Setting up requires restarting the world!.");
        }

        private void Update()
        {
            if (!isWorldLoaded)
            {
                isWorldLoaded = true;
                WorldLoaded();
            }
        }

        private void WorldLoaded()
        {
            StartCoroutine(SetBuildsMul());
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
