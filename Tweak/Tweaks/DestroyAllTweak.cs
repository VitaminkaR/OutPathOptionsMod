using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "DestroyAll", Category = "Resources", ID = 1)]
    public class DestroyAllTweak : Tweak
    {
        private KeyConfigurationElement _key;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[DESTROY ALL]");
            _key = KeyConfigurationElement.Create(GetConfigurations(), Name + "_toggle", "Destroy Key", KeyCode.None);
            ButtonConfigurationElement.Create(GetConfigurations(), Name + "_button", "Destroy").OnChangeValue += Destroy;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_key.Value))
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            foreach (var item in FindObjectsOfType<TakeOutResource>())
            {
                item.TryTakeOut_General(float.MaxValue);
            }
        }
    }
}