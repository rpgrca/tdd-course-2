using System.IO;

namespace com.tenpines.advancetdd
{
    public class CustomerImporter
    {
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

        private void InitializeImport()
        {
            _newCustomer = null;
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
        }

        private void ImportAddress()
        {
            var newAddress = new Address
            {
                StreetName = _currentRecord[1],
                StreetNumber = int.Parse(_currentRecord[2]),
                Town = _currentRecord[3],
                ZipCode = int.Parse(_currentRecord[4]),
                Province = _currentRecord[5]
            };

            _newCustomer.AddAddress(newAddress);
        }

        private void ImportCustomer()
        {
            _newCustomer = new Customer
            {
                FirstName = _currentRecord[1],
                LastName = _currentRecord[2],
                IdentificationType = _currentRecord[3],
                IdentificationNumber = _currentRecord[4]
            };

            _dataBase.Session.Persist(_newCustomer);
        }

        private bool ReadNextLine()
        {
            _currentLine = _lineReader.ReadLine();
            return _currentLine != null;
        }

        private bool IsAddressRecord()
        {
            return _currentLine.StartsWith("A");
        }

        private bool IsCustomerRecord()
        {
            return _currentLine.StartsWith("C");
        }

        private void CreateRecord()
        {
            _currentRecord = _currentLine.Split(',');
        }
    }
}