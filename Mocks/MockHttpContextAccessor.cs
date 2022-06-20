using Microsoft.AspNetCore.Http;

namespace MockLight.Mocks
{
    public class MockHttpContextAccessor : Mock, IHttpContextAccessor
    {
        public HttpContext HttpContext
        {
            get
            {
                return Mocks.HttpContext;
            }
            set
            {
                Mocks.HttpContext = value;
            }
        }
    }
}
