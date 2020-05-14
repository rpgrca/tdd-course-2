using System.Collections.Generic;
using System.IO;
using NHibernate.Linq;

namespace com.tenpines.advancetdd
{
    public class StreamStubBuilder
    {
        private readonly List<string> _lines = new List<string>();

        public StreamStubBuilder AddLine(string line)
        {
            _lines.Add(line);
            return this;
        }

        public StreamReader Build() =>
            new StreamReader(CreateMemoryStreamFrom(_lines));

        private static MemoryStream CreateMemoryStreamFrom(IEnumerable<string> contents)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);

            contents.ForEach(line => streamWriter.WriteLine(line));
            streamWriter.Flush();

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
