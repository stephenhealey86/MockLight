using System;

namespace MockLight
{
    public static class MockOptions
    {
        public static Action MockMethod(Action setup)
        {
            return setup;
        }

        public static Action<TParameter> MockMethod<TParameter>(Action<TParameter> setup)
        {
            return setup;
        }

        public static Func<TResult> MockMethod<TResult>(Func<TResult> setup)
        {
            return setup;
        }

        public static Func<TResult, TParameter> MockMethod<TResult, TParameter>(Func<TResult, TParameter> setup)
        {
            return setup;
        }
    }
}