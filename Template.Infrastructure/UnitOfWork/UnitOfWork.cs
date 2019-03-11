using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.Repositories.ConfigurationRepo;
using Template.Infrastructure.Repositories.ConfigurationRepo.Contracts;
using Template.Infrastructure.Repositories.SessionRepo;
using Template.Infrastructure.Repositories.SessionRepo.Contracts;
using Template.Infrastructure.Repositories.UserRepo;
using Template.Infrastructure.Repositories.UserRepo.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;

namespace Template.Infrastructure.UnitOfWork
{
    public class UnitOfWork : BaseUow, IUnitOfWork
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        private IConfigurationRepo _configurationRepo;
        private ISessionRepo _sessionRepo;
        private IUserRepo _userRepo;

        private bool _disposed;
        private readonly ConnectionStringSettings _connectionSettings;

        #endregion

        #region Constructor

        public UnitOfWork(ConnectionStringSettings connectionStrings, bool beginTransaction = true) : base(connectionStrings)
        {

            _connectionSettings = connectionStrings;

            var connString = DefaultUowConnectionString;

            // Setup Connection & Transaction
            _connection = new SqlConnection(connString);
            _connection.Open();

            if (beginTransaction)
                _transaction = _connection.BeginTransaction();
        }

        #endregion

        #region Properties

        public IConfigurationRepo ConfigurationRepo
        {
            get { return _configurationRepo ?? (_configurationRepo = new ConfigurationRepo(_connection, _transaction, _connectionSettings)); }
        }

        public ISessionRepo SessionRepo
        {
            get { return _sessionRepo ?? (_sessionRepo = new SessionRepo(_connection, _transaction, _connectionSettings)); }
        }

        public IUserRepo UserRepo
        {
            get { return _userRepo ?? (_userRepo = new UserRepo(_connection, _transaction, _connectionSettings)); }
        }

        #endregion

        #region Public Methods

        public bool Commit()
        {
            if (_transaction == null)
            {
                ResetRepositories();
                return true;
            }

            try
            {
                _transaction.Commit();

                return true;
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
                ResetRepositories();
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private void ResetRepositories()
        {
            _configurationRepo = null;
            _sessionRepo = null;
            _userRepo = null;
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            dispose(false);
        }

        #endregion
    }
}
