using System;
using System.Collections.Generic;
using System.Text;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.UnitOfWork.Contracts;

namespace Template.Infrastructure.UnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private ConnectionStringSettings _connectionSettings;

        public UnitOfWorkFactory(ConnectionStringSettings connectionSettings) : base()
        {
            _connectionSettings = connectionSettings;
        }

        public IUnitOfWork GetUnitOfWork(bool beginTransaction = true)
        {
            return new UnitOfWork(_connectionSettings, beginTransaction);
        }
    }
}
