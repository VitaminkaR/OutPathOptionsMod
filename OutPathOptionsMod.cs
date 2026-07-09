using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using OutPathOptionsMod.Configuration;
using OutPathOptionsMod.Tweaks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//TODO:
// World:
// Spawnrate

namespace OutPathOptionsMod
{
    [BepInPlugin("com.vrcompany.outpath.optionsmod", "OutPath Options Mod", "0.2.1")]
    public class OutPathOptionsMod : BaseUnityPlugin
    {
        public PluginInfo PluginInfo { get; } = new PluginInfo();

        private static Harmony harmony = new Harmony("com.vrcompany.outpath.optionsmod");

        public static List<Tweak> Tweaks;

        private ConfigurationHandler configurationHandler;
        public ConfigurationHandler ConfigurationHandler => configurationHandler;

        private static ManualLogSource logger;
        public static ManualLogSource GetLogger => logger;

        private void Awake()
        {
            logger = Logger;

            Logger.LogInfo($"Plugin OPPtionsMod is loaded!");

            configurationHandler = ConfigurationHandler.Create(this, "TWEAKS MENU", KeyCode.M);
            configurationHandler.AddConfigureCategory("Player");
            configurationHandler.AddConfigureCategory("Builds");

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

            Logger.LogInfo("Patching...");

            harmony.PatchAll();

            Logger.LogInfo("Patching done!");
        }
    }
}
