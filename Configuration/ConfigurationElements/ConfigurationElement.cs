using BepInEx.Configuration;

namespace OutPathOptionsMod.Configuration.ConfigurationElements
{
    public abstract class ConfigurationElement
    {
        public const float DEFAULT_BUTTON_SIZE = 64;
        public const float SLIDER_MIN_WIDTH = 128;
        public const float SLIDER_MAX_WIDTH = 384;

        protected ConfigFile _config;

        public bool IsEnabled { get; private set; } = true;

        private ConfigEntry<bool> _isEnabledEntry;

        public void Enable()
        {
            _isEnabledEntry.Value = true;
            IsEnabled = true;
        }

        public void Disable()
        {
            _isEnabledEntry.Value = false;
            IsEnabled = false;
        }

        public ConfigurationElement(string id)
        {
            _config = ConfigurationHandler.Config;
            _isEnabledEntry = _config.Bind<bool>("Toggle", $"{id}_enabled", true);
            IsEnabled = _isEnabledEntry.Value;
        }

        public abstract void Draw();

        public virtual void Update() { }
    }
}
