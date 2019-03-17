using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Template.Infrastructure.Session.Contracts;

namespace Template.Infrastructure.Session
{
    public class SessionProvider : ISessionProvider
    {
        private readonly ISession _session;

        public SessionProvider(
            IHttpContextAccessor _httpContextAccessor
        )
        {
            _session = _httpContextAccessor.HttpContext.Session;
        }

        public bool TryGet<T>(string id, out T value)
        {
            value = default;
            var storedValue = _session.GetString(id);
            if (!string.IsNullOrEmpty(storedValue))
            {
                try
                {
                    value = JsonConvert.DeserializeObject<T>(storedValue);
                    return true;
                }
                catch { }
            }
            return false;
        }

        public T Set<T>(string key, T value)
        {
            if (value != null)
            {
                _session.SetString(key, JsonConvert.SerializeObject(value));
                return value;
            }
            return value;
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }
    }
}
