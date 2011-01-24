using System.Configuration;

namespace NConfig
{
    internal interface IConfigurationRepository
    {
        Configuration GetFileConfiguration(string fileName);
    }
}
