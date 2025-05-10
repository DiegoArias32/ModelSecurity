namespace Utilities.Interfaces
{
    public interface IServiceFactory
    {
        IService<TEntity, TDto> CreateService<TEntity, TDto>()
            where TEntity : class
            where TDto : class;
    }
}