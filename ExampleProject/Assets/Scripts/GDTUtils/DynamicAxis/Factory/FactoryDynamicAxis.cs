using System;
using GDTUtils.Patterns.Factory;

namespace GDTUtils
{
    public class FactoryDynamicAxis : IConcreteFactory<IDynamicAxis>, IDisposable
    {
        public IDynamicAxis Produce()
        {
            IDynamicAxis result = new DynamicAxis.DynamicAxis();
            return result;
        }

        IFactoryProduct IFactory.Produce()
        {
            return Produce();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}