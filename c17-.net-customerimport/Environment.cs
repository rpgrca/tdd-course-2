namespace com.tenpines.advancetdd
{
    public static class Environment
    {
        public static TranscientCustomerService CreateCustomerService() =>
            new TranscientCustomerService();
    }
}
