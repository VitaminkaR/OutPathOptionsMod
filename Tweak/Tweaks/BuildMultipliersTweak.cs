using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "BuildMultipliers", Category = "Builds")]
    public class BuildMultipliersTweak : Tweak
    {
        private static BoolConfigurationElement _toggleSpeed;
        private static FloatConfigurationElement _speedMultiplier;
        private static BoolConfigurationElement _toggleRadius;
        private static FloatConfigurationElement _radiusMultiplier;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[MULTIPLIERS]");
            _toggleSpeed = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_speedToggle", "Toggle Speed Multiplier", false);
            _speedMultiplier = FloatConfigurationElement.Create(GetConfigurations(), $"{Name}_speedMultiplier", "Speed", 1, 0, 1000);
            _speedMultiplier.IsEnabled = _toggleSpeed.Value;
            _toggleSpeed.OnChangeValue += v => _speedMultiplier.IsEnabled = v;
            _toggleRadius = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_radiusToggle", "Toggle Radius Multiplier", false);
            _radiusMultiplier = FloatConfigurationElement.Create(GetConfigurations(), $"{Name}_radiusMultiplier", "Radius", 1, 0, 1000);
            _radiusMultiplier.IsEnabled = _toggleRadius.Value;
            _toggleRadius.OnChangeValue += v => _radiusMultiplier.IsEnabled = v;

#if DEBUG
            BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_generator", "Generator (FOR DEVELOPER! DONT USE!)", false)
                .OnChangeValue += _ => PatchGenerate();
#endif
        }

#if DEBUG
        public void PatchGenerate()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(GameManager));
            var types = assembly.GetTypes();

            Logger.LogInfo("Start BuildMultipliersTweak PatchGenerator");

            using (StreamWriter sw = new StreamWriter(@".\BuildMultipliersTweak_PatchGenerator.txt"))
            {
                sw.WriteLine("#region PATCHES");
                foreach (var type in types)
                {
                    if (!type.Name.StartsWith("Build_")) continue;

                    Logger.LogInfo($"\t{type.Name} generate...");

                    var methods = type
                        .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Where(m => m.Name.Equals("Update"));
                    if (!methods.Any())
                    {
                        Logger.LogInfo($"\t{type.Name} skipped (not find update)!");
                        continue;
                    }

                    var fields = type.GetFields().
                        Where(f => f.Name.StartsWith("_timeTo") ||
                        f.Name.StartsWith("_timeOf") ||
                        f.Name.StartsWith("timeTo") ||
                        f.Name.StartsWith("timeOf") ||
                        f.Name.StartsWith("radius"))
                        .ToArray();
                    if (!fields.Any())
                    {
                        Logger.LogInfo($"\t{type.Name} skipped (not find fields)!");
                        continue;
                    }

                    sw.WriteLine($"[HarmonyPatch(typeof({type.Name}), \"Update\")]");
                    string n = type.Name.Replace("Build_", "");
                    sw.WriteLine($"private static class BuildMultipliersPatches_{n}");
                    sw.WriteLine("{");
                    sw.WriteLine($"\tprivate static void Prefix({type.Name} __instance)");
                    sw.WriteLine("\t{");

                    List<FieldInfo> radiusFields = new List<FieldInfo>(); // fore generation postfix (return the radius value)
                    int fieldsCount = fields.Length;
                    for (int i = 0; i < fieldsCount; i++)
                    {
                        var field = fields[i];

                        Logger.LogInfo($"\t\t{type.Name} {field.Name} prefix generate...");

                        if (field.FieldType != typeof(float))
                        {
                            Logger.LogInfo($"\t\t{type.Name} {field.Name} prefix skipped ({field.FieldType.Name} isnt float)!");
                            continue;
                        }

                        if (field.Name.StartsWith("radius"))
                        {
                            sw.WriteLine("\t\tif (_toggleRadius.Value)");
                            sw.WriteLine($"\t\t\t__instance.{field.Name} *= _radiusMultiplier.Value;");
                            radiusFields.Add(field);
                        }
                        else // _timeTo or _timeOf
                        {
                            sw.WriteLine("\t\tif (_toggleSpeed.Value)");
                            sw.WriteLine($"\t\t\t__instance.{field.Name} -= Time.deltaTime * (_speedMultiplier.Value - 1);");
                        }

                        if (i != fieldsCount - 1)
                            sw.Write("\n");

                        Logger.LogInfo($"\t\t{type.Name} {field.Name} prefix generate successful!");
                    }

                    int radiusFieldsCount = radiusFields.Count;
                    if (radiusFieldsCount > 0)
                    {
                        sw.WriteLine("\t}\n");
                        sw.WriteLine($"\tprivate static void Postfix({type.Name} __instance)");
                        sw.WriteLine("\t{");

                        for (int i = 0; i < radiusFieldsCount; i++)
                        {
                            var field = radiusFields[i];

                            Logger.LogInfo($"\t\t{type.Name} {field.Name} postfix generate...");

                            sw.WriteLine("\t\tif (_toggleRadius.Value)");
                            sw.WriteLine($"\t\t\t__instance.{field.Name} /= _radiusMultiplier.Value;");

                            if (i != fieldsCount - 1)
                                sw.Write("\n");

                            Logger.LogInfo($"\t\t{type.Name} {field.Name} postfix generate successful!");
                        }
                    }

                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                    sw.Write("\n");
                    Logger.LogInfo($"\t{type.Name} generate successful!");
                }
                sw.WriteLine("#endregion PATCHES");
            }
        }
#endif

        #region PATCHES
        [HarmonyPatch(typeof(Build_AutoFeeder), "Update")]
        private static class BuildMultipliersPatches_AutoFeeder
        {
            private static void Prefix(Build_AutoFeeder __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToFeed -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_AutoFeeder __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_AutoFeederIsland), "Update")]
        private static class BuildMultipliersPatches_AutoFeederIsland
        {
            private static void Prefix(Build_AutoFeederIsland __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToFeed -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_AutoFeederIsland __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_BeeHive), "Update")]
        private static class BuildMultipliersPatches_BeeHive
        {
            private static void Prefix(Build_BeeHive __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToWork -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Breaker), "Update")]
        private static class BuildMultipliersPatches_Breaker
        {
            private static void Prefix(Build_Breaker __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToBreak -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Breaker __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_BreakerManual), "Update")]
        private static class BuildMultipliersPatches_BreakerManual
        {
            private static void Prefix(Build_BreakerManual __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToSearch -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToBreak -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_BreakerManual __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_CauldronOfferings), "Update")]
        private static class BuildMultipliersPatches_CauldronOfferings
        {
            private static void Prefix(Build_CauldronOfferings __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToClone -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToClone -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_CauldronSpirits), "Update")]
        private static class BuildMultipliersPatches_CauldronSpirits
        {
            private static void Prefix(Build_CauldronSpirits __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToClone -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToClone -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllConverter), "Update")]
        private static class BuildMultipliersPatches_ChlorophyllConverter
        {
            private static void Prefix(Build_ChlorophyllConverter __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToBreak -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_ChlorophyllConverter __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllExtractor), "Update")]
        private static class BuildMultipliersPatches_ChlorophyllExtractor
        {
            private static void Prefix(Build_ChlorophyllExtractor __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToBreak -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_ChlorophyllExtractor __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_ChlorophyllExtractorManual), "Update")]
        private static class BuildMultipliersPatches_ChlorophyllExtractorManual
        {
            private static void Prefix(Build_ChlorophyllExtractorManual __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToExtract -= Time.deltaTime * (_speedMultiplier.Value - 1);

            }
        }

        [HarmonyPatch(typeof(Build_Clicker), "Update")]
        private static class BuildMultipliersPatches_Clicker
        {
            private static void Prefix(Build_Clicker __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;

                if (_toggleSpeed.Value)
                    __instance.timeToSearch -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }

            private static void Postfix(Build_Clicker __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_Cloner), "Update")]
        private static class BuildMultipliersPatches_Cloner
        {
            private static void Prefix(Build_Cloner __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToClone -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_CollectionNet), "Update")]
        private static class BuildMultipliersPatches_CollectionNet
        {
            private static void Prefix(Build_CollectionNet __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToCollect -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Collector), "Update")]
        private static class BuildMultipliersPatches_Collector
        {
            private static void Prefix(Build_Collector __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToCollect -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Collector __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_CollectorInv), "Update")]
        private static class BuildMultipliersPatches_CollectorInv
        {
            private static void Prefix(Build_CollectorInv __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToCollect -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_CollectorInv __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_CollectorVoid), "Update")]
        private static class BuildMultipliersPatches_CollectorVoid
        {
            private static void Prefix(Build_CollectorVoid __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToCollect -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_CollectorVoid __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_Core), "Update")]
        private static class BuildMultipliersPatches_Core
        {
            private static void Prefix(Build_Core __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToSpawn -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToSpawn -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Core __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_CrabTrap), "Update")]
        private static class BuildMultipliersPatches_CrabTrap
        {
            private static void Prefix(Build_CrabTrap __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToWork -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Craft), "Update")]
        private static class BuildMultipliersPatches_Craft
        {
            private static void Prefix(Build_Craft __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToCraft -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToCraft -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Devourer), "Update")]
        private static class BuildMultipliersPatches_Devourer
        {
            private static void Prefix(Build_Devourer __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToAttack -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Devourer __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_Electrifier), "Update")]
        private static class BuildMultipliersPatches_Electrifier
        {
            private static void Prefix(Build_Electrifier __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToSearch -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToCharge -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Electrifier __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_ElectrifierManual), "Update")]
        private static class BuildMultipliersPatches_ElectrifierManual
        {
            private static void Prefix(Build_ElectrifierManual __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToSearch -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToCharge -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;

                if (_toggleRadius.Value)
                    __instance.radiusToPickFuel *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_ElectrifierManual __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

                if (_toggleRadius.Value)
                    __instance.radiusToPickFuel /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_Fuser), "Update")]
        private static class BuildMultipliersPatches_Fuser
        {
            private static void Prefix(Build_Fuser __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeOfFuse -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance.timeOfFuse -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Incubator), "Update")]
        private static class BuildMultipliersPatches_Incubator
        {
            private static void Prefix(Build_Incubator __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToWork -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Market), "Update")]
        private static class BuildMultipliersPatches_Market
        {
            private static void Prefix(Build_Market __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToResetTrades -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_OreExtractor), "Update")]
        private static class BuildMultipliersPatches_OreExtractor
        {
            private static void Prefix(Build_OreExtractor __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToExtract -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Planter), "Update")]
        private static class BuildMultipliersPatches_Planter
        {
            private static void Prefix(Build_Planter __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToFeed -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Planter __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_ReceptionTower), "Update")]
        private static class BuildMultipliersPatches_ReceptionTower
        {
            private static void Prefix(Build_ReceptionTower __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radiusToPickWater *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_ReceptionTower __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radiusToPickWater /= _radiusMultiplier.Value;
            }
        }

        [HarmonyPatch(typeof(Build_Research), "Update")]
        private static class BuildMultipliersPatches_Research
        {
            private static void Prefix(Build_Research __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToCraft -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToCraft -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_ResourceAmplifier), "Update")]
        private static class BuildMultipliersPatches_ResourceAmplifier
        {
            private static void Prefix(Build_ResourceAmplifier __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radiusToPickWater *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_ResourceAmplifier __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radiusToPickWater /= _radiusMultiplier.Value;
            }
        }

        [HarmonyPatch(typeof(Build_SlayerManual), "Update")]
        private static class BuildMultipliersPatches_SlayerManual
        {
            private static void Prefix(Build_SlayerManual __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToAttack -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_SlayerManual __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_SoilMiner), "Update")]
        private static class BuildMultipliersPatches_SoilMiner
        {
            private static void Prefix(Build_SoilMiner __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToDrill -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_SoilMinerV2), "Update")]
        private static class BuildMultipliersPatches_SoilMinerV2
        {
            private static void Prefix(Build_SoilMinerV2 __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToDrill -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Spreader), "Update")]
        private static class BuildMultipliersPatches_Spreader
        {
            private static void Prefix(Build_Spreader __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToPlant -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Spreader __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_Supplier), "Update")]
        private static class BuildMultipliersPatches_Supplier
        {
            private static void Prefix(Build_Supplier __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radiusToPicItems *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Supplier __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radiusToPicItems /= _radiusMultiplier.Value;
            }
        }

        [HarmonyPatch(typeof(Build_Throttle), "Update")]
        private static class BuildMultipliersPatches_Throttle
        {
            private static void Prefix(Build_Throttle __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;

                if (_toggleRadius.Value)
                    __instance.radiusToPickWater *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Throttle __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

                if (_toggleRadius.Value)
                    __instance.radiusToPickWater /= _radiusMultiplier.Value;
            }
        }

        [HarmonyPatch(typeof(Build_TradePost), "Update")]
        private static class BuildMultipliersPatches_TradePost
        {
            private static void Prefix(Build_TradePost __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToResetTrades -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_Trapper), "Update")]
        private static class BuildMultipliersPatches_Trapper
        {
            private static void Prefix(Build_Trapper __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToAttack -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleSpeed.Value)
                    __instance._timeToAttack -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_Trapper __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_Vaporizer), "Update")]
        private static class BuildMultipliersPatches_Vaporizer
        {
            private static void Prefix(Build_Vaporizer __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToDrill -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump), "Update")]
        private static class BuildMultipliersPatches_WaterPump
        {
            private static void Prefix(Build_WaterPump __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToPump -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_WaterPump_Manual), "Update")]
        private static class BuildMultipliersPatches_WaterPump_Manual
        {
            private static void Prefix(Build_WaterPump_Manual __instance)
            {
                if (_toggleSpeed.Value)
                    __instance._timeToPump -= Time.deltaTime * (_speedMultiplier.Value - 1);
            }
        }

        [HarmonyPatch(typeof(Build_WearStation), "Update")]
        private static class BuildMultipliersPatches_WearStation
        {
            private static void Prefix(Build_WearStation __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToAttack -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_WearStation __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }

        [HarmonyPatch(typeof(Build_WearStationVoid), "Update")]
        private static class BuildMultipliersPatches_WearStationVoid
        {
            private static void Prefix(Build_WearStationVoid __instance)
            {
                if (_toggleSpeed.Value)
                    __instance.timeToAttack -= Time.deltaTime * (_speedMultiplier.Value - 1);

                if (_toggleRadius.Value)
                    __instance.radius *= _radiusMultiplier.Value;
            }

            private static void Postfix(Build_WearStationVoid __instance)
            {
                if (_toggleRadius.Value)
                    __instance.radius /= _radiusMultiplier.Value;

            }
        }
        #endregion PATCHES
    }
}