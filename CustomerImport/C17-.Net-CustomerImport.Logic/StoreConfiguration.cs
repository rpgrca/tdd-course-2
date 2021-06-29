using System;
using FluentNHibernate.Automapping;

namespace CustomerImport.Logic
{
    public class StoreConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type == typeof(Customer) || type == typeof(Address);
        }
    }
}
