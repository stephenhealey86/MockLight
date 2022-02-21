using System;
using Microsoft.Extensions.Options;

namespace MockLight.Mocks
{
    public class OptionsMock<T> : Mock, IOptions<T> where T : class
    {
        public T Value => Mocks.Value;
    }

    public class OptionsMonitorMock<T> : Mock, IOptionsMonitor<T> where T : class
    {
        public T CurrentValue => Mocks.CurrentValue;

        public T Get(string name)
        {
            return Mocks.Get(name);
        }

        public IDisposable OnChange(Action<T, string> listener)
        {
            return Mocks.OnChange(listener);
        }
    }

    public class OptionsSnapshotMock<T> : Mock, IOptionsSnapshot<T> where T : class
    {
        public T Value => Mocks.Value;

        public T Get(string name)
        {
            return Mocks.Get(name);
        }
    }
}