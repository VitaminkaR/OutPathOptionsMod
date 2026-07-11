using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "DupeItem", Category = "Player")]
    public class DupeItemTweak : Tweak
    {
        private static KeyConfigurationElement _key;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[DUPE ITEM]");
            _key = KeyConfigurationElement.Create(GetConfigurations(), Name + "_toggle", "Dupe Key", KeyCode.X);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_key.Value))
                InventoryManager.instance.AddItemToInv(InventoryManager.instance.selectedHotbarSlot.itemInfo, 1);
        }
    }
}