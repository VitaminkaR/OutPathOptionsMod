using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "Minions", Category = "Player")]
    public class MinionsTweak : Tweak
    {
        private static BoolConfigurationElement _toggleInfinite;
        private static BoolConfigurationElement _toggleMax;
        private static IntConfigurationElement _maxCount;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[MINIONS]");
            _toggleInfinite = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_infinite", "Toggle Infinite", false);
            _toggleMax = BoolConfigurationElement.Create(GetConfigurations(), $"{Name}_max", "Toggle Max Count", false);
            _maxCount = IntConfigurationElement.Create(GetConfigurations(), $"{Name}_count", "Max Count", 5, 0, 99999, false);
            _maxCount.IsEnabled = _toggleMax.Value;

            _toggleMax.OnChangeValue += b =>
            {
                _maxCount.IsEnabled = b;
                if (b)
                {
                    SummonsManager.instance.maxSummons = _maxCount.Value;
                }
                else
                {
                    SummonsManager.instance.maxSummons = 5;
                }
            };

            _maxCount.OnChangeValue += i =>
            {
                if (_toggleMax.Value)
                {
                    SummonsManager.instance.maxSummons = i;
                }
                else
                {
                    SummonsManager.instance.maxSummons = 5;
                }
            };
        }

        [HarmonyPatch(typeof(PetAI_MinionCrafter), "Update")]
        private static class MinionsPatches_Crafter
        {
            private static void Postfix(PetAI_MinionCrafter __instance)
            {
                if (_toggleInfinite.Value)
                    __instance.summonTimeAlive = 5;
            }
        }

        [HarmonyPatch(typeof(PetAI_MinionWorker), "Update")]
        private static class MinionsPatches_Worker
        {
            private static void Postfix(PetAI_MinionWorker __instance)
            {
                if (_toggleInfinite.Value)
                    __instance.summonTimeAlive = 5;
            }
        }

        [HarmonyPatch(typeof(PetAI_Slayer), "Update")]
        private static class MinionsPatches_Slayer
        {
            private static void Postfix(PetAI_Slayer __instance)
            {
                if (_toggleInfinite.Value)
                    __instance.summonTimeAlive = 5;
            }
        }

        [HarmonyPatch(typeof(PetAI_SandBeattle), "Update")]
        private static class MinionsPatches_SandBeattle
        {
            private static void Postfix(PetAI_SandBeattle __instance)
            {
                if (_toggleInfinite.Value)
                    __instance.summonTimeAlive = 5;
            }
        }
    }
}