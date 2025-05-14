// Archivo: Data/FormData.cs
using Entity.Model;
using Entity.Contexts;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class FormData : GenericData<Form>
    {
        // Constructor simplificado
        public FormData(ApplicationDbContext context, ILogger<FormData> logger) 
            : base(context, logger)
        {
        }
        
        // No necesitas sobrescribir métodos a menos que requieras comportamiento específico
    }
}