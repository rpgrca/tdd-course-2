using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace com.tenpines.advancetdd
{
    public class DataBase
    {
        public static ISession NewConnection()
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

        public static void BeginTransaction(ISession session, out ITransaction transaction)
        {
            transaction = session.BeginTransaction();
        }

        public static void EndTransaction(ITransaction transaction)
        {
            transaction.Commit();
        }
    }
}
