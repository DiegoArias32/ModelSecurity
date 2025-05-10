using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Microsoft.Extensions.Logging;
using Utilities.Interfaces;

namespace Utilities.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private bool _disposed = false;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            // Check if the repository is cached
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IRepository<T>)_repositories[typeof(T)];
            }

            // Create a new repository
            var loggerType = typeof(ILogger<>).MakeGenericType(typeof(Repository<T>));
            var logger = _serviceProvider.GetService(loggerType) as ILogger<Repository<T>>;
            
            var repository = new Repository<T>(_context, logger);
            
            // Cache the repository
            _repositories.Add(typeof(T), repository);
            
            return repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}