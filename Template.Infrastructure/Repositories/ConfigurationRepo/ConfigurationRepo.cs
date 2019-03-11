using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Adapters;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.Repositories.ConfigurationRepo.Contracts;
using Template.Infrastructure.Repositories.ConfigurationRepo.Models;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.ConfigurationRepo
{
    public class ConfigurationRepo : BaseSQLRepo, IConfigurationRepo
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #endregion

        #region Constructor

        public ConfigurationRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStrings)
            : base(connectionStrings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        #region Public Methods

        public async Task<List<ConfigurationItem>> GetConfigurationItems()
        {
            var sqlStoredProc = "sp_configuration_items_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ConfigurationItem>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new
                    {

                    },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task UpdateConfigurationItem(UpdateConfigurationItemRequest request)
        {
            var sqlStoredProc = "sp_configuration_item_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new DynamicParameters(request),
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items have been updated");
            }
        }

        #endregion
    }
}
