using System.Collections.Generic;

namespace com.tenpines.advancetdd
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