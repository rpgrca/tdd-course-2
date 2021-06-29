using System.Collections.Generic;

namespace CustomerImport.Logic
{
    public class Customer
    {
        public virtual long Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string IdentificationType { get; set; }
        public virtual string IdentificationNumber { get; set; }
        public virtual IList<Address> Addresses { get; set; }

        public Customer()
        {
            Addresses = new List<Address>();
        }

        public virtual void AddAddress(Address anAddress)
        {
            Addresses.Add(anAddress);
        }
    }
}