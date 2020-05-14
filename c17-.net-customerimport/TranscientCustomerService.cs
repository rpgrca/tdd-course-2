using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace com.tenpines.advancetdd
{
    [ExcludeFromCodeCoverage]
    public class TranscientCustomerService : ICustomerService
    {
        private readonly List<Customer> _customers = new List<Customer>();
        private int _transactionCounter = 0;

        public void BeginTransaction() =>
            _transactionCounter++;

        public void EndTransaction() =>
            _transactionCounter--;

        public void Close()
        {
            _customers.Clear();
            _transactionCounter = 0;
        }

        public IList<Customer> GetCustomers() => _customers;

        public Customer GetCustomerWithIdentification(string identificationType, string identificationNumber) =>
            _customers.Single(c =>
                c.IdentificationType == identificationType && c.IdentificationNumber == identificationNumber);

        public void SaveCustomer(Customer newCustomer) => _customers.Add(newCustomer);
    }
}
