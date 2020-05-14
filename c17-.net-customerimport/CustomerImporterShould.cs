using System;
using Xunit;

namespace com.tenpines.advancetdd
{
    public class CustomerImporterShould
    {
        [Fact]
        public void GivenAnImporter_WhenInitializingWithNullDatabase_ThenAnExceptionIsThrown()
        {
            var streamReader = StreamStubBuilder.GetStreamReaderWithNoData();
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
            var streamReader = StreamStubBuilder.GetStreamReaderWithNoData();
            var dataBase = new DataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            customerImporter.Import();

            var customers = dataBase.Session.CreateCriteria(typeof(Customer)).List<Customer>();
            Assert.Empty(customers);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithAddressBeforeCustomer_ThenAnExceptionIsThrown()
        {
            var streamReader = StreamStubBuilder.GetStreamReaderWithAddressBeforeCustomer();
            var dataBase = new DataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.CUSTOMER_IS_NULL_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingUnrecognizedRecord_ThenAnExceptionIsThrown()
        {
            var streamReader = StreamStubBuilder.GetStreamReaderWithCustomerWithEmptyFields();
            var dataBase = new DataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.RECORD_IS_UNRECOGNIZED_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithIncompleteCustomer_ThenAnExceptionIsThrown()
        {
            var streamReader = StreamStubBuilder.GetStreamReaderWithCustomerWithFourFields();
            var dataBase = new DataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithCustomerWithMoreFields_ThenAnExceptionIsThrown()
        {
            var streamReader = StreamStubBuilder.GetStreamReaderWithCustomerWithTenFields();
            var dataBase = new DataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithIncompleteAddress_ThenAnExceptionIsThrown()
        {
            var streamReader = StreamStubBuilder.GetStreamReaderWithAddressWithFourFields();
            var dataBase = new DataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }

        [Fact]
        public void GivenAnImporter_WhenImportingStreamWithAddressWithMoreFields_ThenAnExceptionIsThrown()
        {
            var streamReader = StreamStubBuilder.GetStreamReaderWithAddressWithTenFields();
            var dataBase = new DataBase();
            var customerImporter = new CustomerImporter(dataBase, streamReader);

            var exception = Assert.Throws<ArgumentException>(() => customerImporter.Import());
            Assert.Equal(CustomerImporter.FIELD_AMOUNT_IS_INVALID_EXCEPTION, exception.Message);
        }
    }
}
