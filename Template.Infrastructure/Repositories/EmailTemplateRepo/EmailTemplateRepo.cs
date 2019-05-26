using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Adapters;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.Repositories.Contracts;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.EmailTemplateRepo
{
    public class EmailTemplateRepo : BaseSQLRepo, IEmailTemplateRepo
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #endregion

        #region Constructor

        public EmailTemplateRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStrings)
            : base(connectionStrings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        #region Public Methods

        public async Task<List<EmailTemplateEntity>> GetEmailTemplates()
        {
            var sqlStoredProc = "sp_email_templates_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EmailTemplateEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        #endregion
    }
}
