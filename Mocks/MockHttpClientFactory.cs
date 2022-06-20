using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MockLight.Mocks
{
    public class MockHttpClientFactory : IHttpClientFactory
    {
        Dictionary<string, ClientMock> clients;

        public MockHttpClientFactory()
        {
            MockReset();
        }

        /// <summary>
        /// Resets the mock clients object
        /// </summary>
        /// <returns><see cref="void"/></returns>
        public void MockReset()
        {
            clients = new Dictionary<string, ClientMock>();
        }

        /// <summary>
        /// Creates a <see cref="HttpClient"/> with a <see cref="MockHttpMessageHandler"/> for the given <paramref name="name"/>
        /// </summary>
        /// <param name="name">A <see cref="string"/>that is the name of the client</param>
        /// <returns><see cref="HttpClient"/></returns>
        public HttpClient CreateClient(string name)
        {
            if (!clients.ContainsKey(name))
            {
                var handler =  new MockHttpMessageHandler();
                var clientMock = new ClientMock
                {
                    Client = new HttpClient(handler),
                    Handler = handler
                };
                clients.Add(name, clientMock);
            }
            return clients[name].Client;
        }

        /// <summary>
        /// Sets up the <see cref="MockHttpMessageHandler"/> to return the given <see cref="Task<HttpResponseMessage>>"/>
        /// </summary>
        /// <param name="setup">A <see cref="Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>"/> which will be called by the <see cref="HttpMessageHandler"/> SendAsync method</param>
        /// <returns><see cref="void"/></returns>
        public void MockSetup(string name, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> setup)
        {
            if (!clients.ContainsKey(name))
            {
                var handler =  new MockHttpMessageHandler();
                var clientMock = new ClientMock
                {
                    Client = new HttpClient(handler),
                    Handler = handler
                };
                clients.Add(name, clientMock);
            }
            clients[name].Handler.MockSetup(setup);
        }

        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _mock;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _mock(request, cancellationToken);
            }
            public void MockSetup(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> setup)
            {
                _mock = setup;
            }
        }

        private class ClientMock
        {
            public HttpClient Client { get; set; }
            public MockHttpMessageHandler Handler { get; set; }
        }
    }
}
