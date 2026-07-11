using OutPathOptionsMod.Configuration.ConfigurationElements;

namespace OutPathOptionsMod.Tweaks
{
    [Tweak(Name = "MaxStats", Category = "Player")]
    public class MaxStatsTweak : Tweak
    {
        private BoolConfigurationElement _toggle;

        public override void Init(OutPathOptionsMod plugin)
        {
            base.Init(plugin);

            HeaderConfigurationElement.Create(GetConfigurations(), $"{Name}_header", "[INFINITE STATS]");
            _toggle = BoolConfigurationElement.Create(GetConfigurations(), Name, "Toggle", false);
        }

        private void Update()
        {
            var player = PlayerGarden.instance;
            if (player != null && _toggle.Value)
            {
                float hsim = player.healthyStateIncreaseMult;

                player.AddStat_Food(float.MaxValue);
                player.AddStat_Health(float.MaxValue);
                player.AddStat_Stamina(float.MaxValue);
                player.AddStat_Energy(float.MaxValue);

                player.healthyStateIncreaseMult = hsim;
            }
        }
    }
}