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
            var customerService = new TranscientCustomerService();
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

            var customerService = new TranscientCustomerService();
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

            var customerService = new TranscientCustomerService();
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

            var customerService = new TranscientCustomerService();
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

            var customerService = new TranscientCustomerService();
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

            var customerService = new TranscientCustomerService();
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

            var customerService = new TranscientCustomerService();
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

            var customerService = new TranscientCustomerService();
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

            var customerService = new TranscientCustomerService();
            var customerImporter = new CustomerImporter(customerService, streamReader);

            Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Single(customerService.GetCustomers());
            Assert.Empty(customerService.GetCustomers().Single().Addresses);
        }
    }
}
