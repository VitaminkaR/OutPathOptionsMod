using OutPathOptionsMod.Configuration.ConfigurationElements;
using UnityEngine;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "KillAll", Category = "Resources")]
    public class KillAllTweak : Tweak
    {
        private KeyConfigurationElement _key;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[KILL ALL]");
            _key = KeyConfigurationElement.Create(GetConfigurations(), Name + "_toggle", "Kill Key", KeyCode.None);
            ButtonConfigurationElement.Create(GetConfigurations(), Name + "_button", "Kill").OnChangeValue += Kill;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_key.Value))
            {
                Kill();
            }
        }

        private void Kill()
        {
            foreach (var item in FindObjectsOfType<EnemyHealth>())
            {
                item.DamageEnemy_General(float.MaxValue);
            }
        }
    }
}