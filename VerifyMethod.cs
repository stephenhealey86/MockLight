using System.Collections.Generic;

namespace MockLight
{
    public class VerifyMethod
    {
        public int Count { get; set; }
        public IDictionary<int, IEnumerable<object>> Parameters { get; set; }
        public VerifyMethod()
        {
            Count = 1;
            Parameters = new Dictionary<int, IEnumerable<object>>();
        }

        public VerifyMethod(IEnumerable<object> parameters)
        {
            Count = 1;
            Parameters = new Dictionary<int, IEnumerable<object>>();
            AddParameters(parameters);
        }

        public void AddParameters(IEnumerable<object> parameters)
        {
            Parameters.Add(Count, parameters);
        }

        public void ClearCalls()
        {
            Count = 0;
            Parameters = new Dictionary<int, IEnumerable<object>>();
        }
    }
}
