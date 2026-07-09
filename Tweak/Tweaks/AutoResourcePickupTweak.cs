using HarmonyLib;
using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "AutoResourcePickup", Category = "Resources", ID = 0)]
    public class AutoResourcePickupTweak : Tweak
    {
        private static BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[AUTO PICKUP]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        [HarmonyPatch(typeof(ItemPrefab), "SetupItemInfo")]
        private static class AutoResourcePickupPatches
        {
            private static void Postfix(ItemPrefab __instance)
            {
                if (_toggle.Value)
                    __instance.DirectCollect();
            }
        }
    }
}