using OutPathOptionsMod.Configuration.ConfigurationElements;
using System.Collections.Generic;

namespace OutPathOptionsMod.Configuration
{
    public interface IConfigureObject
    {
        int GetID();
        string GetCategory();
        List<ConfigurationElement> GetConfigurations();
    }
}
