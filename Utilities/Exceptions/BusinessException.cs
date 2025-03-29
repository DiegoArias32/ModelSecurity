using System;

namespace Utilities.Exceptions
{
    /// <summary>
    /// Excepción base para todos los errores relacionados con la capa de negocio.
    /// </summary>
    public class BusinessException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="BusinessException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public BusinessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="BusinessException"/> con un mensaje de error y una excepción interna.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando no se encuentra una entidad en el sistema.
    /// </summary>
    public class EntityNotFoundException : BusinessException
    {
        /// <summary>
        /// Tipo de entidad que no se encontró.
        /// </summary>
        public string EntityType { get; }

        /// <summary>
        /// Identificador de la entidad buscada.
        /// </summary>
        public object EntityId { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="EntityNotFoundException"/> con un mensaje personalizado.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public EntityNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="EntityNotFoundException"/> con información de la entidad.
        /// </summary>
        /// <param name="entityType">Tipo de entidad que no se encontró.</param>
        /// <param name="entityId">Identificador de la entidad buscada.</param>
        public EntityNotFoundException(string entityType, object entityId)
            : base($"La entidad '{entityType}' con ID '{entityId}' no fue encontrada.")
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="EntityNotFoundException"/> con información detallada.
        /// </summary>
        /// <param name="entityType">Tipo de entidad que no se encontró.</param>
        /// <param name="entityId">Identificador de la entidad buscada.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public EntityNotFoundException(string entityType, object entityId, Exception innerException)
            : base($"La entidad '{entityType}' con ID '{entityId}' no fue encontrada.", innerException)
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }

    /// <summary>
    /// Excepción lanzada cuando los datos proporcionados no cumplen con las reglas de validación.
    /// </summary>
    public class ValidationException : BusinessException
    {
        /// <summary>
        /// Campo que no cumple con la validación.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ValidationException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe el error de validación.</param>
        public ValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ValidationException"/> con información del campo inválido.
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que falló la validación.</param>
        /// <param name="message">Mensaje que describe el error de validación.</param>
        public ValidationException(string propertyName, string message)
            : base($"Error de validación en '{propertyName}': {message}")
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ValidationException"/> con información detallada.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando se intenta realizar una operación que viola reglas de negocio.
    /// </summary>
    public class BusinessRuleViolationException : BusinessException
    {
        /// <summary>
        /// Código que identifica la regla de negocio violada.
        /// </summary>
        public string RuleCode { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="BusinessRuleViolationException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe la violación de la regla de negocio.</param>
        public BusinessRuleViolationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="BusinessRuleViolationException"/> con un código de regla.
        /// </summary>
        /// <param name="ruleCode">Código que identifica la regla de negocio violada.</param>
        /// <param name="message">El mensaje que describe la violación.</param>
        public BusinessRuleViolationException(string ruleCode, string message)
            : base($"Violación de regla de negocio [{ruleCode}]: {message}")
        {
            RuleCode = ruleCode;
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="BusinessRuleViolationException"/> con información detallada.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public BusinessRuleViolationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando se intenta acceder a un recurso sin los permisos adecuados.
    /// </summary>
    public class UnauthorizedAccessBusinessException : BusinessException
    {
        /// <summary>
        /// Recurso al que se intentó acceder.
        /// </summary>
        public string Resource { get; }

        /// <summary>
        /// Tipo de operación que se intentó realizar.
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UnauthorizedAccessBusinessException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe el error de autorización.</param>
        public UnauthorizedAccessBusinessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UnauthorizedAccessBusinessException"/> con información detallada.
        /// </summary>
        /// <param name="resource">Recurso al que se intentó acceder.</param>
        /// <param name="operation">Operación que se intentó realizar.</param>
        public UnauthorizedAccessBusinessException(string resource, string operation)
            : base($"Acceso no autorizado al recurso '{resource}' para la operación '{operation}'.")
        {
            Resource = resource;
            Operation = operation;
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UnauthorizedAccessBusinessException"/> con información detallada.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public UnauthorizedAccessBusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando ocurre un error al interactuar con recursos externos como bases de datos o servicios.
    /// </summary>
    public class ExternalServiceException : BusinessException
    {
        /// <summary>
        /// Nombre del servicio externo que generó el error.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ExternalServiceException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe el error del servicio externo.</param>
        public ExternalServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ExternalServiceException"/> con información del servicio.
        /// </summary>
        /// <param name="serviceName">Nombre del servicio externo.</param>
        /// <param name="message">El mensaje que describe el error.</param>
        public ExternalServiceException(string serviceName, string message)
            : base($"Error en el servicio externo '{serviceName}': {message}")
        {
            ServiceName = serviceName;
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ExternalServiceException"/> con información detallada.
        /// </summary>
        /// <param name="serviceName">Nombre del servicio externo.</param>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public ExternalServiceException(string serviceName, string message, Exception innerException)
            : base($"Error en el servicio externo '{serviceName}': {message}", innerException)
        {
            ServiceName = serviceName;
        }
    }
}