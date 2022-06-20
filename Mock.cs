using System;
using System.Collections.Generic;
using System.Dynamic;

namespace MockLight
{
    /// <summary>
    /// This abstract class provides mocking functionality to classes that implement it. To create a mock class of an interface you
    /// create a mock class that inheits from this class as well as the interface it is mocking.
    /// </summary>
    /// <example>
    /// Please see below for demo of how to mock an interface.
    /// <code>
    /**
        // Interface to mock
        public interface IAccount
        {
            bool PayBill(int amount);
            Task<Statement> GetStatementAsync();
            void Verify();
        }
        // Mock class
        public class MockAccount : Mock, IAccount
        {
            // Getters
            public Person AccountHolder => Mocks.AccountHolder;

            // Methods from interface
            public bool PayBill(int amount)
            {
                return Mocks.PayBill(amount);
            }
            public Task<Statement> GetStatementAsync()
            {
                return Mocks.GetStatementAsync();
            }
            public void Verify()
            {
                Mocks.Verify();
            }
        }
    */
    /// </code>
    /// </example>
    /// <example>
    /// Please see below for demo of how to change mocked methods and getters
    /// <code>
    /**
        public class TestClassMocking
        {
            private MockAccount mockAccount = new MockAccount();

            public void TestPayBillMethod()
            {
                // Arrange
                bool mockResult = true;
                mockAccount.MockSetup<int, bool>("PayBill", (amount) =>
                {
                    return mockResult;
                });
            }

            public void TestAccountHolderMethod_One()
            {
                // Arrange
                Person mockResult = new Person();
                mockAccount.MockSetup<Person>("AccountHolder", () =>
                {
                    return mockResult;
                });
            }

            public void TestAccountHolderMethod_Two()
            {
                // Arrange
                Person mockResult = new Person();
                mockAccount.MockSetup<Person>("AccountHolder", mockResult);
            }

            // Advanced setup
            public void TestAccountHolderMethod_Advanced()
            {
                // Arrange
                Person mockResult = new Person();
                mockAccount.MockSetup((mocks) =>
                {
                    var name = "AccountHolder";
                    mocks[name] = mockResult;
                    // Manually call UpdateCalls method
                    mockAccount.MockUpdateCalls(name);
                });
            }
        }
    */
    /// </code>
    /// </example>
    /// <example>
    /// Please see below for demo of how to verify mocked methods and getters
    /// <code>
    /**
        public class TestClassVerifying
        {
            private MockAccount mockAccount = new MockAccount();

            public void TestPayBillMethod()
            {
                // Arrange
                bool mockResult = true;
                int argument = 1;
                mockAccount.MockSetup<int, bool>("PayBill", (amount) =>
                {
                    return mockResult;
                });
                // Act
                var result = mockAccount.PayBill(argument);
                // Assert
                var verify = mockAccount.MockVerify("PayBill");
                Assert.IsTrue(verify.HasBeenCalledTimes(1));
            }
        }
    */
    /// </code>
    /// </example>
    public abstract class Mock
    {
        private dynamic _mocks;
        private IDictionary<string, object> _mocker => _mocks;
        protected dynamic Mocks => _mocks;
        private IDictionary<string, VerifyMethod> _calls;
        
        public Mock()
        {
            MockReset();
        }

        /// <summary>
        /// Resets the mock object and call history
        /// </summary>
        /// <returns><see cref="void"/></returns>
        public void MockReset()
        {
            _mocks = new ExpandoObject();
            _calls = new Dictionary<string, VerifyMethod>();
        }

        /// <summary>
        /// Exposes the mock object for complete control of setting up a mock.
        /// You will need to update the calls object manually when using this method if you whish to use the <see cref="VerifyObject"/> object afterwards.
        /// </summary>
        /// <param name="setup">An <see cref="Action"/> that takes the mock object as a parameter.</param>
        /// <returns><see cref="void"/></returns>
        public void MockSetup(Action<dynamic> setup)
        {
            setup(_mocks);
        }

        /// <summary>
        /// Sets the mock object property for the given <paramref name="name" /> to the given <paramref name="obj" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the property you are mocking.</param>
        /// <param name="obj">The value you are assigning to the mock property.</param>
        /// <returns><see cref="void"/></returns>
        public void MockSetup<T>(string name, T obj)
        {
            _mocker[name] = obj;
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Action"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Action"/> that will be called when the mock method is called.</param>
        public void MockSetup(string name, Action setup)
        {
            _mocker[name] = (Action)(() => {
                MockUpdateCalls(name);
                setup();
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Action{TParameter}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Action{TParameter}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TParameter">The type of parameter the mock method takes.</typeparam>
        public void MockSetup<TParameter>(string name, Action<TParameter> setup)
        {
            _mocker[name] = (Action<TParameter>)((parameter) => {
                MockUpdateCalls(name, parameter);
                setup(parameter);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Action{TP1, TP2}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Action{TP1, TP2}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TP1">The type of the first parameter the mock method takes.</typeparam>
        /// <typeparam name="TP2">The type of the second parameter the mock method takes.</typeparam>
        public void MockSetup<TP1, TP2>(string name, Action<TP1, TP2> setup)
        {
            _mocker[name] = (Action<TP1, TP2>)((p1, p2) => {
                MockUpdateCalls(name, p1, p2);
                setup(p1, p2);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Action{TP1, TP2, TP3}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Action{TP1, TP2, TP3}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TP1">The type of the first parameter the mock method takes.</typeparam>
        /// <typeparam name="TP2">The type of the second parameter the mock method takes.</typeparam>
        /// <typeparam name="TP3">The type of the third parameter the mock method takes.</typeparam>
        public void MockSetup<TP1, TP2, TP3>(string name, Action<TP1, TP2, TP3> setup)
        {
            _mocker[name] = (Action<TP1, TP2, TP3>)((p1, p2, p3) => {
                MockUpdateCalls(name, p1, p2, p3);
                setup(p1, p2, p3);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Action{TP1, TP2, TP3, TP4}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Action{TP1, TP2, TP3, TP4}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TP1">The type of the first parameter the mock method takes.</typeparam>
        /// <typeparam name="TP2">The type of the second parameter the mock method takes.</typeparam>
        /// <typeparam name="TP3">The type of the third parameter the mock method takes.</typeparam>
        /// <typeparam name="TP4">The type of the forth parameter the mock method takes.</typeparam>
        public void MockSetup<TP1, TP2, TP3, TP4>(string name, Action<TP1, TP2, TP3, TP4> setup)
        {
            _mocker[name] = (Action<TP1, TP2, TP3, TP4>)((p1, p2, p3, p4) => {
                MockUpdateCalls(name, p1, p2, p3, p4);
                setup(p1, p2, p3, p4);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Action{TP1, TP2, TP3, TP4, TP5}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Action{TP1, TP2, TP3, TP4, TP5}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TP1">The type of the first parameter the mock method takes.</typeparam>
        /// <typeparam name="TP2">The type of the second parameter the mock method takes.</typeparam>
        /// <typeparam name="TP3">The type of the third parameter the mock method takes.</typeparam>
        /// <typeparam name="TP4">The type of the forth parameter the mock method takes.</typeparam>
        /// <typeparam name="TP5">The type of the fifth parameter the mock method takes.</typeparam>
        public void MockSetup<TP1, TP2, TP3, TP4, TP5>(string name, Action<TP1, TP2, TP3, TP4, TP5> setup)
        {
            _mocker[name] = (Action<TP1, TP2, TP3, TP4, TP5>)((p1, p2, p3, p4, p5) => {
                MockUpdateCalls(name, p1, p2, p3, p4, p5);
                setup(p1, p2, p3, p4, p5);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Func{TResult}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Func{TResult}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TResult">The type of object the mock will return.</typeparam>
        /// <returns><paramref name="TResult"/></returns>
        public void MockSetup<TResult>(string name, Func<TResult> setup)
        {
            _mocker[name] = (Func<TResult>)(() => {
                MockUpdateCalls(name);
                return setup();
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Func{TParameter, TResult}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Func{TParameter, TResult}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TParameter">The type of parameter the mock method takes.</typeparam>
        /// <typeparam name="TResult">The type of object the mock will return.</typeparam>
        /// <returns><paramref name="TResult"/></returns>
        public void MockSetup<TParameter, TResult>(string name, Func<TParameter, TResult> setup)
        {
            _mocker[name] = (Func<TParameter, TResult>)((parameter) => {
                MockUpdateCalls(name, parameter);
                return setup(parameter);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Func{TP1, TP2, TResult}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Func{TP1, TP2, TResult}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TP1">The type of the first parameter the mock method takes.</typeparam>
        /// <typeparam name="TP2">The type of the second parameter the mock method takes.</typeparam>
        /// <typeparam name="TResult">The type of object the mock will return.</typeparam>
        /// <returns><paramref name="TResult"/></returns>
        public void MockSetup<TP1, TP2, TResult>(string name, Func<TP1, TP2, TResult> setup)
        {
            _mocker[name] = (Func<TP1, TP2, TResult>)((p1, p2) => {
                MockUpdateCalls(name, p1, p2);
                return setup(p1, p2);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Func{TP1, TP2, TP3, TResult}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Func{TP1, TP2, TP3, TResult}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TP1">The type of the first parameter the mock method takes.</typeparam>
        /// <typeparam name="TP2">The type of the second parameter the mock method takes.</typeparam>
        /// <typeparam name="TP3">The type of the third parameter the mock method takes.</typeparam>
        /// <typeparam name="TResult">The type of object the mock will return.</typeparam>
        /// <returns><paramref name="TResult"/></returns>
        public void MockSetup<TP1, TP2, TP3, TResult>(string name, Func<TP1, TP2, TP3, TResult> setup)
        {
            _mocker[name] = (Func<TP1, TP2, TP3, TResult>)((p1, p2, p3) => {
                MockUpdateCalls(name, p1, p2, p3);
                return setup(p1, p2, p3);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Func{TP1, TP2, TP3, TP4, TResult}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Func{TP1, TP2, TP3, TP4, TResult}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TP1">The type of the first parameter the mock method takes.</typeparam>
        /// <typeparam name="TP2">The type of the second parameter the mock method takes.</typeparam>
        /// <typeparam name="TP3">The type of the third parameter the mock method takes.</typeparam>
        /// <typeparam name="TP4">The type of the forth parameter the mock method takes.</typeparam>
        /// <typeparam name="TResult">The type of object the mock will return.</typeparam>
        /// <returns><paramref name="TResult"/></returns>
        public void MockSetup<TP1, TP2, TP3, TP4, TResult>(string name, Func<TP1, TP2, TP3, TP4, TResult> setup)
        {
            _mocker[name] = (Func<TP1, TP2, TP3, TP4, TResult>)((p1, p2, p3, p4) => {
                MockUpdateCalls(name, p1, p2, p3, p4);
                return setup(p1, p2, p3, p4);
            });
        }

        /// <summary>
        /// Sets the mock object method for the given <paramref name="name" /> to the given <see cref="Func{TP1, TP2, TP3, TP4, TP5, TResult}"/> <paramref name="setup" />.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the method you are mocking.</param>
        /// <param name="setup">The <see cref="Func{TP1, TP2, TP3, TP4, TP5, TResult}"/> that will be called when the mock method is called.</param>
        /// <typeparam name="TP1">The type of the first parameter the mock method takes.</typeparam>
        /// <typeparam name="TP2">The type of the second parameter the mock method takes.</typeparam>
        /// <typeparam name="TP3">The type of the third parameter the mock method takes.</typeparam>
        /// <typeparam name="TP4">The type of the forth parameter the mock method takes.</typeparam>
        /// <typeparam name="TP5">The type of the fifth parameter the mock method takes.</typeparam>
        /// <typeparam name="TResult">The type of object the mock will return.</typeparam>
        /// <returns><paramref name="TResult"/></returns>
        public void MockSetup<TP1, TP2, TP3, TP4, TP5, TResult>(string name, Func<TP1, TP2, TP3, TP4, TP5, TResult> setup)
        {
            _mocker[name] = (Func<TP1, TP2, TP3, TP4, TP5, TResult>)((p1, p2, p3, p4, p5) => {
                MockUpdateCalls(name, p1, p2, p3, p4, p5);
                return setup(p1, p2, p3, p4, p5);
            });
        }

        /// <summary>
        /// Gets the <see cref="VerifyObject"/> object associated with the <paramref name="name"/> mock.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the mock you want to verify.</param>
        /// <returns><see cref="VerifyObject"/></returns>
        public VerifyObject MockVerify(string name)
        {
            if (_calls.ContainsKey(name) && _calls[name] is VerifyMethod verify)
            {
                return new VerifyObject(verify);
            }
            else
            {
                throw new Exception($"Setup has not been called for {name}.");
            }
        }

        /// <summary>
        /// Updates the calls object for the given <paramref name="name"/> with the given <paramref name="parameterList"/>.
        /// </summary>
        /// <param name="name">A <see cref="string"/> that is the name of the mock you are updating</param>
        /// <param name="parameterList">The parameters that the mock was called with.</param>
        /// <returns>void</returns>
        public void MockUpdateCalls(string name, params object[] parameterList)
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
