using System;
using Template.Infrastructure.Repositories.ConfigurationRepo.Contracts;
using Template.Infrastructure.Repositories.SessionRepo.Contracts;
using Template.Infrastructure.Repositories.UserRepo.Contracts;

namespace Template.Infrastructure.UnitOfWork.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IConfigurationRepo ConfigurationRepo { get; }

        ISessionRepo SessionRepo { get; }

        IUserRepo UserRepo { get; }

        bool Commit();
    }
}
