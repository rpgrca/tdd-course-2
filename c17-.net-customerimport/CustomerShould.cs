using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace com.tenpines.advancetdd
{
    public class CustomerShould
    {
        [Fact]
        public void Test1()
        {
            Customer.ImportCustomers();
        }
    }
}
