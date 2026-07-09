using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "SpawnRate", Category = "Resources", ID = 3)]
    public class SpawnRateTweak : Tweak
    {
        private static BoolConfigurationElement _toggleProp;
        private static FloatConfigurationElement _propSR;
        private static BoolConfigurationElement _toggleEnemy;
        private static FloatConfigurationElement _enemySR;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[SPAWN RATE]");
            _toggleProp = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_toggle_propsr", "Toggle Prop Spawn Rate", false);
            _propSR = FloatConfigurationElement.Create(GetConfigurations(), $"{Name}_propsr", "Prop Spawn Rate", 2, 1, 100);
            _propSR.IsEnabled = _toggleProp.Value;
            _toggleProp.OnChangeValue += (bool v) => _propSR.IsEnabled = v;

            _toggleEnemy = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_toggle_enemysr", "Toggle Enemy Spawn Rate", false);
            _enemySR = FloatConfigurationElement.Create(GetConfigurations(), $"{Name}_enemysr", "Enemy Spawn Rate", 2, 1, 100);
            _enemySR.IsEnabled = _toggleEnemy.Value;
            _toggleEnemy.OnChangeValue += (bool v) => _enemySR.IsEnabled = v;
        }

        [HarmonyPatch(typeof(IsleGenerator), "Update")]
        private static class SpawnRatePatches_Prop
        {
            private static void Prefix(IsleGenerator __instance)
            {
                if (_toggleProp.Value)
                    __instance.timeToSpawnProp -= Time.deltaTime * (_propSR.Value - 1);

                if (_toggleEnemy.Value)
                    __instance._timeToSpawnCreature -= Time.deltaTime * (_enemySR.Value - 1);
            }
        }
    }
}