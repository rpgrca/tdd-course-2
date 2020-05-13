using System;
using System.IO;
using System.Linq;
using NHibernate.Criterion;
using Xunit;

namespace com.tenpines.advancetdd
{
    public class CustomerShould : IDisposable
    {
        private readonly StreamReader _streamReader;
        private readonly CustomerImporter _customerImporter;
        private readonly DataBase _dataBase;

        public CustomerShould()
        {
            _streamReader = new StreamReader(new FileStream("input.txt", FileMode.Open));
            _dataBase = new DataBase();
            _customerImporter = new CustomerImporter(_dataBase, _streamReader);
        }

        [Fact]
        public void GivenAnEmptyDatabase_WhenImportingSampleData_TwoCustomersAreImported()
        {
            _customerImporter.Import();

            var customers = _dataBase.Session.CreateCriteria(typeof(Customer)).List<Customer>();
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void GivenImportedCustomers_WhenQueryingIdNumber22333444_ThenACompleteCustomerWithTwoAddressesIsFound()
        {
            _customerImporter.Import();

            var customer = _dataBase.Session
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
            _customerImporter.Import();

            var customer = _dataBase.Session
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

        public void Dispose()
        {
            _streamReader.Close();
            _dataBase.Close();
        }
    }
}
