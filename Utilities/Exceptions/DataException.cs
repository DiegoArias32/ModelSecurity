using System;

namespace Utilities.Exceptions
{
    /// <summary>
    /// Excepción base para todos los errores relacionados con la capa de datos.
    /// </summary>
    public class DataException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DataException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public DataException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DataException"/> con un mensaje de error y una excepción interna.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public DataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando ocurre un error de conexión con la base de datos.
    /// </summary>
    public class DatabaseConnectionException : DataException
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DatabaseConnectionException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe el error de conexión.</param>
        public DatabaseConnectionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DatabaseConnectionException"/> con un mensaje de error y una excepción interna.
        /// </summary>
        /// <param name="message">El mensaje que describe el error de conexión.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando ocurre un error al ejecutar una consulta en la base de datos.
    /// </summary>
    public class QueryExecutionException : DataException
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="QueryExecutionException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe el error de consulta.</param>
        public QueryExecutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="QueryExecutionException"/> con un mensaje de error y una excepción interna.
        /// </summary>
        /// <param name="message">El mensaje que describe el error de consulta.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public QueryExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando ocurre un conflicto de concurrencia en la base de datos.
    /// </summary>
    public class ConcurrencyException : DataException
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ConcurrencyException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe el error de concurrencia.</param>
        public ConcurrencyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ConcurrencyException"/> con un mensaje de error y una excepción interna.
        /// </summary>
        /// <param name="message">El mensaje que describe el error de concurrencia.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando se violan restricciones de integridad en la base de datos.
    /// </summary>
    public class DataIntegrityException : DataException
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DataIntegrityException"/> con un mensaje de error.
        /// </summary>
        /// <param name="message">El mensaje que describe la violación de integridad.</param>
        public DataIntegrityException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DataIntegrityException"/> con un mensaje de error y una excepción interna.
        /// </summary>
        /// <param name="message">El mensaje que describe la violación de integridad.</param>
        /// <param name="innerException">La excepción que es la causa del error actual.</param>
        public DataIntegrityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}