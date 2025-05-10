namespace Utilities.Interfaces
{
    public interface IValidationStrategy<T>
    {
        bool IsValid(T entity);
        string GetErrorMessage();
    }
}