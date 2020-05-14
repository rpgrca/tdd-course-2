using System.IO;
using NHibernate.Linq;

namespace com.tenpines.advancetdd
{
    public class StreamStubBuilder
    {
        private static class MemoryStreamBuilder
        {
            public static MemoryStream CreateFrom(string[] contents)
            {
                var memoryStream = new MemoryStream();
                var streamWriter = new StreamWriter(memoryStream);

                contents.ForEach(line => streamWriter.WriteLine(line));
                streamWriter.Flush();

                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        public static StreamReader GetStreamReaderWithCorrectData() =>
            new StreamReader(MemoryStreamBuilder.CreateFrom(new []
            {
                "C,Pepe,Sanchez,D,22333444",
                "A,San Martin,3322,Olivos,1636,BsAs",
                "A,Maipu,888,Florida,1122,Buenos Aires",
                "C,Juan,Perez,C,23-25666777-9",
                "A,Alem,1122,CABA,1001,CABA"
            }));

        public static StreamReader GetStreamReaderWithAddressBeforeCustomer() =>
            new StreamReader(MemoryStreamBuilder.CreateFrom(new[]
            {
                "A,Alem,1122,CABA,1001,CABA",
                "C,Juan,Perez,C,23-25666777-9"
            }));

        public static StreamReader GetStreamReaderWithNoData() =>
            new StreamReader(new MemoryStream());

        public static StreamReader GetStreamReaderWithCustomerWithEmptyFields() =>
            new StreamReader(MemoryStreamBuilder.CreateFrom(new[]
            {
                ",,,,,"
            }));

        public static StreamReader GetStreamReaderWithCustomerWithFourFields() =>
            new StreamReader(MemoryStreamBuilder.CreateFrom(new []
            {
                "C,Juan,Perez,C"
            }));

        public static StreamReader GetStreamReaderWithAddressWithFourFields() =>
            new StreamReader(MemoryStreamBuilder.CreateFrom(new[]
            {
                "C,Juan,Perez,C,23-25666777-9",
                "A,Alem,1122,CABA,1001"
            }));
    }
}
