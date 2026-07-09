using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "GetItems", Category = "Player", ID = 5)]
    public class GetItemsTweak : Tweak
    {
        private static KeyConfigurationElement _key;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[GET ITEMS]");
            _key = KeyConfigurationElement.Create(GetConfigurations(), Name + "_toggle", "Get Key", KeyCode.C);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_key.Value))
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
}