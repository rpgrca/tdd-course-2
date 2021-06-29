using System.Collections.Generic;

namespace CustomerImport.Logic
{
    public interface ICustomerService
    {
        void BeginTransaction();
        void EndTransaction();
        void Close();
        IList<Customer> GetCustomers();
        Customer GetCustomerWithIdentification(string identificationType, string identificationNumber);
        void SaveCustomer(Customer newCustomer);
    }
}