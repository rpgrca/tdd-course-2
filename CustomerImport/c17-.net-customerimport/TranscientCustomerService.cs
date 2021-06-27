using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace com.tenpines.advancetdd
{
    [ExcludeFromCodeCoverage]
    public class TranscientCustomerService : ICustomerService
    {
        private readonly List<Customer> _customers;

        public TranscientCustomerService() => _customers = new List<Customer>();

        public void BeginTransaction()
        {
        }

        public void EndTransaction()
        {
        }

        public void Close() => _customers.Clear();

        public IList<Customer> GetCustomers() => _customers;

        public Customer GetCustomerWithIdentification(string identificationType, string identificationNumber) =>
            _customers.Single(c =>
                c.IdentificationType == identificationType && c.IdentificationNumber == identificationNumber);

        public void SaveCustomer(Customer newCustomer) => _customers.Add(newCustomer);
    }
}
