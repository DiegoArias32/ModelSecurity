using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Utilities.Interfaces;
using Utilities.Services;

namespace Utilities.Factories
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactory(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IService<TEntity, TDto> CreateService<TEntity, TDto>()
            where TEntity : class
            where TDto : class
        {
            // Try to get a specialized service
            var serviceType = typeof(IService<TEntity, TDto>);
            var service = _serviceProvider.GetService(serviceType) as IService<TEntity, TDto>;
            
            if (service != null)
            {
                return service;
            }

            // Create a generic service
            var loggerType = typeof(ILogger<>).MakeGenericType(typeof(Service<TEntity, TDto>));
            var logger = _serviceProvider.GetService(loggerType) as ILogger<Service<TEntity, TDto>>;
            
            if (logger == null)
            {
                throw new InvalidOperationException($"No se pudo resolver la dependencia {loggerType.Name}");
            }
            
            return new Service<TEntity, TDto>(_unitOfWork, _mapper, logger);
        }
    }
}