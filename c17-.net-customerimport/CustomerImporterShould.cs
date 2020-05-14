using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace com.tenpines.advancetdd
{
    public class CustomerImporterShould
    {
        [Fact]
        public void GivenAnImporter_WhenInitializingWithNullCustomerService_ThenAnExceptionIsThrown()
        {
            var streamReader = new StreamStubBuilder().Build();
            var exception = Assert.Throws<ArgumentException>(() => new CustomerImporter(null, streamReader));
            Assert.Equal(CustomerImporter.CUSTOMER_SERVICE_IS_NULL_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenInitializingWithNullStreamReader_ThenAnExceptionIsThrown()
        {
            var customerService = new PersistentCustomerService();
            var exception = Assert.Throws<ArgumentException>(() => new CustomerImporter(customerService, null));
            Assert.Equal(CustomerImporter.STREAM_READER_IS_NULL_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingFromEmptyStream_ThenNoCustomerIsImported()
        {
            var streamReader = new StreamStubBuilder().Build();
            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            customerImporter.Import();

            var customers = customerService.GetCustomers();
            Assert.Empty(customers);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithAddressBeforeCustomer_ThenAnExceptionIsThrown()
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("A,Alem,1122,CABA,1001,CABA")
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.CUSTOMER_IS_NULL_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithAddressBeforeCustomer_ThenNoCustomerIsImported()
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("A,Alem,1122,CABA,1001,CABA")
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Empty(customerService.GetCustomers());
        }

        [Theory]
        [MemberData(nameof(GetUnrecognizedRecordSamples))]
        public void GivenAnImporter_WhenImportingUnrecognizedRecord_ThenAnExceptionIsThrown(string unrecognizedRecord)
        {
            var streamReader = new StreamStubBuilder()
                .AddLine(unrecognizedRecord)
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.RECORD_IS_UNRECOGNIZED_EXCEPTION, exception.Message);
        }

        public static IEnumerable<object[]> GetUnrecognizedRecordSamples() =>
            new[]
            {
                "Z,,,,,",
                null,
                string.Empty,
                "Carlos,Juan,Perez,C,23-25666777-9",
                "Alberto,Alem,1122,CABA,1001,CABA"
            }.Select(unrecognizedRecord => new object[] {unrecognizedRecord});

        [Theory]
        [MemberData(nameof(GetUnrecognizedRecordSamples))]
        public void GivenAnImporter_WhenImportingUnrecognizedRecord_ThenNoCustomerIsImported(string unrecognizedRecord)
        {
            var streamReader = new StreamStubBuilder()
                .AddLine(unrecognizedRecord)
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Empty(customerService.GetCustomers());
        }

        [Theory]
        [MemberData(nameof(GetInvalidCustomerFieldCountRecords))]
        public void GivenAnImporter_WhenImportingStreamWithCustomerWithInvalidAmountOfFields_ThenAnExceptionIsThrown(string invalidRecord)
        {
            var streamReader = new StreamStubBuilder()
                .AddLine(invalidRecord)
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }

        public static IEnumerable<object[]> GetInvalidCustomerFieldCountRecords() =>
            new[] {"C,Pepe,Sanchez,D,22333444,A,B,C,D,E", "C,Juan,Perez,C"}
                .Select(invalidRecord => new object[] { invalidRecord });

        [Theory]
        [MemberData(nameof(GetInvalidCustomerFieldCountRecords))]
        public void GivenAnImporter_WhenImportingStreamWithCustomerWithInvalidAmountOfFields_ThenNoCustomerIsImported(string invalidRecord)
        {
            var streamReader = new StreamStubBuilder()
                .AddLine(invalidRecord)
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Empty(customerService.GetCustomers());
        }

        [Theory]
        [MemberData(nameof(GetInvalidAddressFieldCountRecords))]
        public void GivenAnImporter_WhenImportingStreamWithIncompleteAddress_ThenAnExceptionIsThrown(string invalidRecord)
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .AddLine(invalidRecord)
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }

        public static IEnumerable<object[]> GetInvalidAddressFieldCountRecords() =>
            new[] { "A,Alem,1122,CABA,1001", "A,Alem,1122,CABA,1001,CABA,A,B,C,D" }
                .Select(invalidRecord => new object[] { invalidRecord });

        [Theory]
        [MemberData(nameof(GetInvalidAddressFieldCountRecords))]
        public void GivenAnImporter_WhenImportingStreamWithIncompleteAddress_ThenACustomerWithoutAddressIsAdded(string invalidRecord)
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .AddLine(invalidRecord)
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Single(customerService.GetCustomers());
            Assert.Empty(customerService.GetCustomers().Single().Addresses);
        }

        [Fact]
        public void GivenAnEmptyDatabase_WhenImportingSampleData_TwoCustomersAreImported()
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("C,Pepe,Sanchez,D,22333444")
                .AddLine("A,San Martin,3322,Olivos,1636,BsAs")
                .AddLine("A,Maipu,888,Florida,1122,Buenos Aires")
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .AddLine("A,Alem,1122,CABA,1001,CABA")
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            customerImporter.Import();

            var customers = customerService.GetCustomers();
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void GivenImportedCustomers_WhenQueryingIdNumber22333444_ThenACompleteCustomerWithTwoAddressesIsFound()
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("C,Pepe,Sanchez,D,22333444")
                .AddLine("A,San Martin,3322,Olivos,1636,BsAs")
                .AddLine("A,Maipu,888,Florida,1122,Buenos Aires")
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .AddLine("A,Alem,1122,CABA,1001,CABA")
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            customerImporter.Import();

            var customer = customerService.GetCustomerWithIdentification("D", "22333444");

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
                    Assert.Equal("BsAs", a1.Province);
                },
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
            var streamReader = new StreamStubBuilder()
                .AddLine("C,Pepe,Sanchez,D,22333444")
                .AddLine("A,San Martin,3322,Olivos,1636,BsAs")
                .AddLine("A,Maipu,888,Florida,1122,Buenos Aires")
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .AddLine("A,Alem,1122,CABA,1001,CABA")
                .Build();

            var customerService = Environment.CreateCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            customerImporter.Import();

            var customer = customerService.GetCustomerWithIdentification("C", "23-25666777-9");

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
    }
}
