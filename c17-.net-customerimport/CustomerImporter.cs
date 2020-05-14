using System;
using System.IO;

namespace com.tenpines.advancetdd
{
    public class CustomerImporter
    {
        public const string CUSTOMER_IS_NULL_EXCEPTION = "Customer is null.";
        public const string RECORD_IS_INCOMPLETE_EXCEPTION = "Record is incomplete.";
        public const string RECORD_IS_UNRECOGNIZED_EXCEPTION = "Record is unrecognized.";

        private readonly DataBase _dataBase;
        private readonly StreamReader _lineReader;
        private string _currentLine;
        private string[] _currentRecord;
        private Customer _newCustomer;

        public CustomerImporter(DataBase dataBase, StreamReader lineReader)
        {
            _dataBase = dataBase;
            _lineReader = lineReader;
        }

        public void Import()
        {
            _dataBase.BeginTransaction();

            InitializeImport();
            while (ReadNextLine())
            {
                CreateRecord();
                ImportRecord();
            }

            _dataBase.EndTransaction();
        }

        private void ImportRecord()
        {
            if (IsCustomerRecord())
            {
                ImportCustomer();
            }
            else if (IsAddressRecord())
            {
                ImportAddress();
            }
            else
            {
                throw new ArgumentException(RECORD_IS_UNRECOGNIZED_EXCEPTION);
            }
        }

        private void ImportAddress()
        {
            _ = _newCustomer ?? throw new ArgumentException(CUSTOMER_IS_NULL_EXCEPTION);
            if (_currentRecord.Length != 6)
            {
                throw new ArgumentException(RECORD_IS_INCOMPLETE_EXCEPTION);
            }

            _newCustomer.AddAddress(new Address
            {
                StreetName = _currentRecord[1],
                StreetNumber = int.Parse(_currentRecord[2]),
                Town = _currentRecord[3],
                ZipCode = int.Parse(_currentRecord[4]),
                Province = _currentRecord[5]
            });
        }

        private void ImportCustomer()
        {
            if (_currentRecord.Length != 5)
            {
                throw new ArgumentException(RECORD_IS_INCOMPLETE_EXCEPTION);
            }

            _newCustomer = new Customer
            {
                FirstName = _currentRecord[1],
                LastName = _currentRecord[2],
                IdentificationType = _currentRecord[3],
                IdentificationNumber = _currentRecord[4]
            };

            _dataBase.Session.Persist(_newCustomer);
        }

        private void InitializeImport() =>
            _newCustomer = null;

        private bool ReadNextLine() =>
            (_currentLine = _lineReader.ReadLine()) != null;

        private bool IsAddressRecord() =>
            _currentLine.StartsWith("A");

        private bool IsCustomerRecord() =>
            _currentLine.StartsWith("C");

        private void CreateRecord() =>
            _currentRecord = _currentLine.Split(',');
    }
}