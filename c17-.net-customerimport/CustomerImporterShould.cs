using System;
using Xunit;

namespace com.tenpines.advancetdd
{
    public class CustomerImporterShould
    {
        [Fact]
        public void GivenAnImporter_WhenInitializingWithNullDatabase_ThenAnExceptionIsThrown()
        {
            var streamReader = new StreamStubBuilder().Build();
            var exception = Assert.Throws<ArgumentException>(() => new CustomerImporter(null, streamReader));
            Assert.Equal(CustomerImporter.DATABASE_IS_NULL_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenInitializingWithNullStreamReader_ThenAnExceptionIsThrown()
        {
            var dataBase = new DataBase();
            var exception = Assert.Throws<ArgumentException>(() => new CustomerImporter(dataBase, null));
            Assert.Equal(CustomerImporter.STREAM_READER_IS_NULL_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingFromEmptyStream_ThenNoCustomerIsImported()
        {
            var streamReader = new StreamStubBuilder().Build();
            var dataBase = new MemoryDataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            customerImporter.Import();

            var customers = dataBase.GetCustomers();
            Assert.Empty(customers);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithAddressBeforeCustomer_ThenAnExceptionIsThrown()
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("A,Alem,1122,CABA,1001,CABA")
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .Build();

            var dataBase = new MemoryDataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.CUSTOMER_IS_NULL_EXCEPTION, exception.Message);
        }

        [Theory]
        [InlineData("Z,,,,,")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Carlos,Juan,Perez,C,23-25666777-9")]
        [InlineData("Alberto,Alem,1122,CABA,1001,CABA")]
        public void GivenAnImporter_WhenImportingUnrecognizedRecord_ThenAnExceptionIsThrown(string unrecognizedRecord)
        {
            var streamReader = new StreamStubBuilder()
                .AddLine(unrecognizedRecord)
                .Build();

            var dataBase = new MemoryDataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.RECORD_IS_UNRECOGNIZED_EXCEPTION, exception.Message);
        }

        [Theory]
        [InlineData("C,Pepe,Sanchez,D,22333444,A,B,C,D,E")]
        [InlineData("C,Juan,Perez,C")]
        public void GivenAnImporter_WhenImportingStreamWithCustomerWithInvalidAmountOfFields_ThenAnExceptionIsThrown(string invalidRecord)
        {
            var streamReader = new StreamStubBuilder()
                .AddLine(invalidRecord)
                .Build();

            var dataBase = new MemoryDataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithIncompleteAddress_ThenAnExceptionIsThrown()
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .AddLine("A,Alem,1122,CABA,1001")
                .Build();

            var dataBase = new MemoryDataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithAddressWithMoreFields_ThenAnExceptionIsThrown()
        {
            var streamReader = new StreamStubBuilder()
                .AddLine("C,Juan,Perez,C,23-25666777-9")
                .AddLine("A,Alem,1122,CABA,1001,CABA,A,B,C,D")
                .Build();

            var dataBase = new MemoryDataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }
    }
}
