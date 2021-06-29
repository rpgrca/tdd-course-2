using CustomerImport.Logic;

namespace CustomerImport.UnitTests
{
    public static class Environment
    {
        public static TranscientCustomerService CreateCustomerService() =>
            new TranscientCustomerService();
    }
}
