using System;
using System.IO;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;
using Xunit;

namespace com.tenpines.advancetdd
{
    public class CustomerShould : IDisposable
    {
        private ISession _session;
        private ITransaction _transaction;

        [Fact]
        public void Test1()
        {
            ImportCustomers();

            var customers = _session.CreateCriteria(typeof(Customer)).List<Customer>();
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void Test2()
        {
            ImportCustomers();

            var customers = _session
                .CreateCriteria(typeof(Customer))
                .Add(Restrictions.Eq("IdentificationType", "D"))
                .Add(Restrictions.Eq("IdentificationNumber", "22333444"))
                .List<Customer>();

            Assert.Single(customers);
            Assert.Equal("Pepe", customers[0].FirstName);
            Assert.Equal("Sanchez", customers[0].LastName);
        }

        public CustomerShould()
        {
            SetUp();
        }

        public void ImportCustomers()
        {
            var fileStream = new System.IO.FileStream("input.txt", FileMode.Open);

            var lineReader = new StreamReader(fileStream);

            Customer newCustomer = null;
            var line = lineReader.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("C"))
                {
                    var customerData = line.Split(',');
                    newCustomer = new Customer();
                    newCustomer.FirstName = customerData[1];
                    newCustomer.LastName = customerData[2];
                    newCustomer.IdentificationType = customerData[3];
                    newCustomer.IdentificationNumber = customerData[4];
                    _session.Persist(newCustomer);
                }
                else if (line.StartsWith("A"))
                {
                    var addressData = line.Split(',');
                    var newAddress = new Address();

                    newCustomer.AddAddress(newAddress);
                    newAddress.StreetName = addressData[1];
                    newAddress.StreetNumber = Int32.Parse(addressData[2]);
                    newAddress.Town = addressData[3];
                    newAddress.ZipCode = Int32.Parse(addressData[4]);
                    newAddress.Province = addressData[3];
                }

                line = lineReader.ReadLine();
            }

            fileStream.Close();
        }

        private void CloseSession()
        {
            _transaction.Commit();
            _session.Close();
        }

        private void SetUp()
        {
            var storeConfiguration = new StoreConfiguration();
            var configuration = Fluently.Configure()
                .Database(MsSqlCeConfiguration.Standard.ShowSql().ConnectionString("Data Source=CustomerImport.sdf"))
                .Mappings(m => m.AutoMappings.Add(AutoMap
                    .AssemblyOf<Customer>(storeConfiguration)
                    .Override<Customer>(map => map.HasMany(x => x.Addresses).Cascade.All())));

            var sessionFactory = configuration.BuildSessionFactory();
            new SchemaExport(configuration.BuildConfiguration()).Execute(true, true, false);
            _session = sessionFactory.OpenSession();
            _transaction = _session.BeginTransaction();
        }

        public void Dispose()
        {
            CloseSession();
            _session?.Dispose();
            _transaction?.Dispose();
        }
    }
}
