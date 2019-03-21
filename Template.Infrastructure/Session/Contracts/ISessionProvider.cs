using System;
using System.Threading.Tasks;

namespace Template.Infrastructure.Session.Contracts
{
    public interface ISessionProvider
    {
        Task<T> Get<T>(string key);

        Task<T> Set<T>(string key, T value);

        Task Remove(string key);
    }
}