using OutPathOptionsMod.Configuration.ConfigurationElements;
using System.Collections.Generic;

namespace OutPathOptionsMod.Configuration
{
    public interface IConfigureObject
    {
        List<ConfigurationElement> GetConfigurations();
    }
}
