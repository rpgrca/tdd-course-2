using System.IO;

namespace com.tenpines.advancetdd
{
    public class CustomerImporter
    {
        private readonly DataBase _dataBase;
        private readonly StreamReader _lineReader;

        public CustomerImporter(DataBase dataBase, StreamReader lineReader)
        {
            _dataBase = dataBase;
            _lineReader = lineReader;
        }

        public void Import()
        {
            Customer newCustomer = null;
            _dataBase.BeginTransaction();
            var line = _lineReader.ReadLine();

            while (line != null)
            {
                if (line.StartsWith("C"))
                {
                    var customerData = line.Split(',');
                    newCustomer = new Customer
                    {
                        FirstName = customerData[1],
                        LastName = customerData[2],
                        IdentificationType = customerData[3],
                        IdentificationNumber = customerData[4]
                    };
                    _dataBase.Session.Persist(newCustomer);
                }
                else if (line.StartsWith("A"))
                {
                    var addressData = line.Split(',');
                    var newAddress = new Address
                    {
                        StreetName = addressData[1],
                        StreetNumber = int.Parse(addressData[2]),
                        Town = addressData[3],
                        ZipCode = int.Parse(addressData[4]),
                        Province = addressData[5]
                    };

                    newCustomer.AddAddress(newAddress);
                }

                line = _lineReader.ReadLine();
            }

            _dataBase.EndTransaction();
        }
    }
}