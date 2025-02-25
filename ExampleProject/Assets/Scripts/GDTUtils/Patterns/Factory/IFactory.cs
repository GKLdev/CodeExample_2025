namespace GDTUtils.Patterns.Factory
{
    public interface IFactory
    {
        IFactoryProduct Produce();
    }

    public interface IConcreteFactory<TProduct> : IFactory where TProduct : IFactoryProduct
    {
        new TProduct Produce();
    }

    public interface IFactoryProduct
    {
        
    }
}
