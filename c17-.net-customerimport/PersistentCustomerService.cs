using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;

namespace com.tenpines.advancetdd
{
    public class PersistentCustomerService : ICustomerService
    {
        // TODO: Transactions should be a pile
        private ITransaction _transaction;
        private readonly ISession _session;

        public PersistentCustomerService() => _session = NewConnection();

        private static ISession NewConnection()
        {
            var storeConfiguration = new StoreConfiguration();
            var configuration = Fluently.Configure()
                .Database(MsSqlCeConfiguration.Standard.ShowSql().ConnectionString("Data Source=CustomerImport.sdf"))
                .Mappings(m => m.AutoMappings.Add(AutoMap
                    .AssemblyOf<Customer>(storeConfiguration)
                    .Override<Customer>(map => map.HasMany(x => x.Addresses).Cascade.All())));

            var sessionFactory = configuration.BuildSessionFactory();
            new SchemaExport(configuration.BuildConfiguration()).Execute(true, true, false);

            return sessionFactory.OpenSession();
        }

        public void BeginTransaction() => _transaction = _session.BeginTransaction();

        public void EndTransaction() => _transaction.Commit();

        public void Close()
        {
            _session.Close();
            _session.Dispose();
        }

        public IList<Customer> GetCustomers() => _session.CreateCriteria<Customer>().List<Customer>();

        public Customer GetCustomerWithIdentification(string identificationType, string identificationNumber) =>
            _session
                .CreateCriteria<Customer>()
                .Add(Restrictions.Eq("IdentificationType", identificationType))
                .Add(Restrictions.Eq("IdentificationNumber", identificationNumber))
                .List<Customer>()
                .Single();

        public void SaveCustomer(Customer newCustomer) => _session.Persist(newCustomer);
    }
}