using System;

namespace Template.Infrastructure.Session.Contracts
{
    public interface ISessionProvider
    {
        bool TryGet<T>(string key, out T value);

        T Set<T>(string key, T value);
    }
}