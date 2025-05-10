using Utilities.Interfaces;

namespace Utilities.Strategies
{
    public class ValidationContext<T>
    {
        private IValidationStrategy<T> _strategy;

        public ValidationContext(IValidationStrategy<T> strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IValidationStrategy<T> strategy)
        {
            _strategy = strategy;
        }

        public bool Validate(T entity)
        {
            return _strategy.IsValid(entity);
        }

        public string GetErrorMessage()
        {
            return _strategy.GetErrorMessage();
        }
    }
}