using System.Collections.Generic;
using System.Linq;

namespace MockLight
{
    public class VerifyObject
    {
        private int _count;
        private IDictionary<int, IEnumerable<object>> _parameters { get; set; }

        public VerifyObject()
        {
            _count = 0; _parameters = new Dictionary<int, IEnumerable<object>>();
        }
        public VerifyObject(int count, IDictionary<int, IEnumerable<object>> parameters)
        {
            _count = count; _parameters = parameters;
        }
        public bool HasBeenCalled() => _count > 0;
        public bool HasBeenCalledTimes(int number) => number == _count;
        public bool HasBeenCalledWith(params object[] parameters)
        {
            return _parameters.Any(p => {
                if(p.Value.Count() == parameters.Count()) {
                    var firstNotSecond = p.Value.Except(parameters).ToList();
                    var secondNotFirst = parameters.Except(p.Value).ToList();
                    return !firstNotSecond.Any() && !secondNotFirst.Any();
                }
                return false;
            });
        }
    }
}