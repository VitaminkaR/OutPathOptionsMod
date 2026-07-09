using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "InstantInteract", Category = "Player", ID = 1)]
    public class InstantInteractTweak : Tweak
    {
        private BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INSTANT INTERACT]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        private void Update()
        {
            var player = PlayerGarden.instance;
            if (player != null && _toggle.Value)
            {
                player.healthyStateIncreaseMult = 0;
            }
        }
    }
}