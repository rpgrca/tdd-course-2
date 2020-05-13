using System;
using System.IO;
using System.Linq;
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
        public void GivenAnEmptyDatabase_WhenImportingSampleData_TwoCustomersAreImported()
        {
            ImportCustomers();

            var customers = _session.CreateCriteria(typeof(Customer)).List<Customer>();
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void GivenImportedCustomers_WhenQueryingIdNumber22333444_ThenACompleteCustomerWithTwoAddressesIsFound()
        {
            ImportCustomers();

            var customer = _session
                .CreateCriteria(typeof(Customer))
                .Add(Restrictions.Eq("IdentificationType", "D"))
                .Add(Restrictions.Eq("IdentificationNumber", "22333444"))
                .List<Customer>()
                .Single();

            Assert.NotNull(customer);
            Assert.Equal("D", customer.IdentificationType);
            Assert.Equal("22333444", customer.IdentificationNumber);
            Assert.Equal("Pepe", customer.FirstName);
            Assert.Equal("Sanchez", customer.LastName);

            Assert.Collection(customer.Addresses,
                a1 =>
                {
                    Assert.Equal("San Martin", a1.StreetName);
                    Assert.Equal(3322, a1.StreetNumber);
                    Assert.Equal("Olivos", a1.Town);
                    Assert.Equal(1636, a1.ZipCode);
                    Assert.Equal("BsAs", a1.Province); },
                a2 =>
                {
                    Assert.Equal("Maipu", a2.StreetName);
                    Assert.Equal(888, a2.StreetNumber);
                    Assert.Equal("Florida", a2.Town);
                    Assert.Equal(1122, a2.ZipCode);
                    Assert.Equal("Buenos Aires", a2.Province);
                });
        }

        [Fact]
        public void GivenImportedCustomers_WhenQueryingIdNumber23256667779_ThenACompleteCustomerWithOneAddressIsFound()
        {
            ImportCustomers();

            var customer = _session
                .CreateCriteria(typeof(Customer))
                .Add(Restrictions.Eq("IdentificationType", "C"))
                .Add(Restrictions.Eq("IdentificationNumber", "23-25666777-9"))
                .List<Customer>()
                .Single();

            Assert.NotNull(customer);
            Assert.Equal("C", customer.IdentificationType);
            Assert.Equal("23-25666777-9", customer.IdentificationNumber);
            Assert.Equal("Juan", customer.FirstName);
            Assert.Equal("Perez", customer.LastName);

            Assert.Collection(customer.Addresses,
                a1 =>
                {
                    Assert.Equal("Alem", a1.StreetName);
                    Assert.Equal(1122, a1.StreetNumber);
                    Assert.Equal("CABA", a1.Town);
                    Assert.Equal(1001, a1.ZipCode);
                    Assert.Equal("CABA", a1.Province);
                });
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
                    newAddress.Province = addressData[5];
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
