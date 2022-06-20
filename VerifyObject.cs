using System;
using System.Linq;

namespace MockLight
{
    public class VerifyObject
    {
        private VerifyMethod _calls;

        public VerifyObject()
        {
            _calls = new VerifyMethod();
        }

        public VerifyObject(VerifyMethod calls)
        {
            _calls = calls;
        }

        /// <summary>
        /// Used to verify if the mock has been called.
        /// </summary>
        /// <returns><see cref="true"/> if the mock has been called otherwise <see cref="false"/>.</returns>
        public bool HasBeenCalled() => _calls.Count > 0;

        /// <summary>
        /// Used to verify if the mock has been called a specific number of times.
        /// </summary>
        /// <param name="number">An <see cref="int"/> that repsents the predicated number of times the mock was called.</param>
        /// <returns><see cref="true"/> if the mock has been called <paramref name="number"/> times otherwise <see cref="false"/>.</returns>
        public bool HasBeenCalledTimes(int number) => number == _calls.Count;

        /// <summary>
        /// Used to verify if the mock has been called with <paramref name="parameters"/>
        /// </summary>
        /// <param name="parameters">The predicted parameters that the mock was called with, checked by reference if object</param>
        /// <returns><see cref="true"/> if the mock has been called with <paramref name="parameters"/> otherwise <see cref="false"/>.</returns>
        public bool HasBeenCalledWith(params object[] parameters)
        {
            return _calls.Parameters.Any(p => {
                if(p.Value.Count() == parameters.Count()) {
                    var firstNotSecond = p.Value.Except(parameters).ToList();
                    var secondNotFirst = parameters.Except(p.Value).ToList();
                    return !firstNotSecond.Any() && !secondNotFirst.Any();
                }
                return false;
            });
        }
        
        /// <summary>
        /// Used to verify if the mock has been called with <paramref name="value"/>
        /// </summary>
        /// <param name="parameters">The predicted parameters that the mock was called with, checked by reference if object unless a <paramref name="comparer"/> is passed.</param>
        /// <param name="index">Am <see cref="int"/> that is the postion of the parameter in the mock method signature.</param>
        /// <param name="value">The predicted value of the parameter.</param>
        /// <param name="comparer">A <see cref="Func{T, T, bool}"/> that takes the parameter at position <paramref name="index"/> and the <paramref name="value"/> and returns a <see cref="bool"/> that represents a match.</param>
        /// <typeparam name="T">The type of <paramref name="value"/></typeparam>
        /// <returns><see cref="true"/> if the mock has been called with <paramref name="value"/> otherwise <see cref="false"/>.</returns>
        public bool HasBeenCalledWithParameter<T>(int index, T value, Func<T, T, bool> comparer = null)
        {
            return _calls.Parameters.Any(x => {
                var parameter = x.Value.ToArray()[index];
                if (parameter.GetType() == value.GetType())
                {
                    if (comparer != null)
                    {
                        return comparer((T)parameter, value);
                    }
                    return parameter.Equals(value);
                }
                return false;
            });
        }
        /// <summary>
        /// Clears the mock call history
        /// </summary>
        /// <returns><see cref="void"/></returns>
        public void ClearCalls()
        {
            _calls.ClearCalls();
        }
    }
}
