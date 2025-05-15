using Entity.Model;
using Entity.Contexts;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class LoginData : GenericData<Login>
    {
        public LoginData(ApplicationDbContext context, ILogger<LoginData> logger)
            : base(context, logger)
        {
        }
        
        // Si necesitas métodos específicos para Login, puedes añadirlos aquí
    }
}