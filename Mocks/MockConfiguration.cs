using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace MockLight.Mocks
{
    public class MockConfiguration : Mock, IConfiguration
    {
        public string this[string key]
        {
            get
            {
                return Mocks[key];
            }
            set
            {
                Mocks[key] = value;
            }
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return Mocks.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            return Mocks.GetReloadToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return Mocks.GetSection(key);
        }
    }
}
