using System.IO;
using NHibernate;

namespace com.tenpines.advancetdd
{
    public class CustomerImporter
    {
        private readonly ISession _session;
        private readonly StreamReader _lineReader;
        private ITransaction _transaction;

        public CustomerImporter(ISession session, StreamReader lineReader)
        {
            _lineReader = lineReader;
            _session = session;
        }

        public void Import()
        {
            Customer newCustomer = null;
            BeginTransaction(_session, out _transaction);
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
                    _session.Persist(newCustomer);
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

            EndTransaction();
        }

        private void EndTransaction()
        {
            _transaction.Commit();
        }

        private void BeginTransaction(ISession session, out ITransaction transaction)
        {
            transaction = session.BeginTransaction();
        }
    }
}