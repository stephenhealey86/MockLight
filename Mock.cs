using System;
using System.Collections.Generic;
using System.Dynamic;

namespace MockLight
{
    public abstract class Mock
    {
        private readonly dynamic _mocks;
        private IDictionary<string, object> _mocker => _mocks;
        public dynamic Mocks => _mocks;
        private IDictionary<string, VerifyMethod> _calls;
        
        public Mock()
        {
            _mocks = new ExpandoObject();
            _calls = new Dictionary<string, VerifyMethod>();
        }

        public void Setup(Action<dynamic> setup)
        {
            setup(_mocks);
        }

        public void Setup(string name, Action setup)
        {
            _mocker[name] = (Action)(() => {
                UpdateCalls(name);
                setup();
            });
        }

        public void Setup<TParameter>(string name, Action<TParameter> setup)
        {
            _mocker[name] = (Action<TParameter>)((parameter) => {
                UpdateCalls(name, parameter);
                setup(parameter);
            });
        }

        public void Setup<TP1, TP2>(string name, Action<TP1, TP2> setup)
        {
            _mocker[name] = (Action<TP1, TP2>)((p1, p2) => {
                UpdateCalls(name, p1, p2);
                setup(p1, p2);
            });
        }

        public void Setup<TP1, TP2, TP3>(string name, Action<TP1, TP2, TP3> setup)
        {
            _mocker[name] = (Action<TP1, TP2, TP3>)((p1, p2, p3) => {
                UpdateCalls(name, p1, p2, p3);
                setup(p1, p2, p3);
            });
        }

        public void Setup<TP1, TP2, TP3, TP4>(string name, Action<TP1, TP2, TP3, TP4> setup)
        {
            _mocker[name] = (Action<TP1, TP2, TP3, TP4>)((p1, p2, p3, p4) => {
                UpdateCalls(name, p1, p2, p3, p4);
                setup(p1, p2, p3, p4);
            });
        }

        public void Setup<TP1, TP2, TP3, TP4, TP5>(string name, Action<TP1, TP2, TP3, TP4, TP5> setup)
        {
            _mocker[name] = (Action<TP1, TP2, TP3, TP4, TP5>)((p1, p2, p3, p4, p5) => {
                UpdateCalls(name, p1, p2, p3, p4, p5);
                setup(p1, p2, p3, p4, p5);
            });
        }

        public void Setup<TResult>(string name, Func<TResult> setup)
        {
            _mocker[name] = (Func<TResult>)(() => {
                UpdateCalls(name);
                return setup();
            });
        }

        public void Setup<TParameter, TResult>(string name, Func<TParameter, TResult> setup)
        {
            _mocker[name] = (Func<TParameter, TResult>)((parameter) => {
                UpdateCalls(name, parameter);
                return setup(parameter);
            });
        }

        public void Setup<TP1, TP2, TResult>(string name, Func<TP1, TP2, TResult> setup)
        {
            _mocker[name] = (Func<TP1, TP2, TResult>)((p1, p2) => {
                UpdateCalls(name, p1, p2);
                return setup(p1, p2);
            });
        }

        public void Setup<TP1, TP2, TP3, TResult>(string name, Func<TP1, TP2, TP3, TResult> setup)
        {
            _mocker[name] = (Func<TP1, TP2, TP3, TResult>)((p1, p2, p3) => {
                UpdateCalls(name, p1, p2, p3);
                return setup(p1, p2, p3);
            });
        }

        public void Setup<TP1, TP2, TP3, TP4, TResult>(string name, Func<TP1, TP2, TP3, TP4, TResult> setup)
        {
            _mocker[name] = (Func<TP1, TP2, TP3, TP4, TResult>)((p1, p2, p3, p4) => {
                UpdateCalls(name, p1, p2, p3, p4);
                return setup(p1, p2, p3, p4);
            });
        }

        public void Setup<TP1, TP2, TP3, TP4, TP5, TResult>(string name, Func<TP1, TP2, TP3, TP4, TP5, TResult> setup)
        {
            _mocker[name] = (Func<TP1, TP2, TP3, TP4, TP5, TResult>)((p1, p2, p3, p4, p5) => {
                UpdateCalls(name, p1, p2, p3, p4, p5);
                return setup(p1, p2, p3, p4, p5);
            });
        }

        public VerifyObject Verify(string name)
        {
            if (_calls.ContainsKey(name) && _calls[name] is VerifyMethod verify)
            {
                return new VerifyObject(verify.Count, verify.Parameters);
            }
            else
            {
                return new VerifyObject();
            }
        }

        public void UpdateCalls(string name, params object[] parameterList)
        {
            if (_calls.ContainsKey(name) && _calls[name] is VerifyMethod verify)
            {
                verify.Count++;
                if (parameterList.Length > 0) {
                    verify.AddParameters(parameterList);
                }
            }
            else
            {
                _calls[name] = parameterList.Length > 0 ? new VerifyMethod(parameterList) : new VerifyMethod();
            }
        }
    }
}
