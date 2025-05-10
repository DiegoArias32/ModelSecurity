// Data/RolData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class RolData : BaseData<Rol>
    {
        public RolData(IRepository<Rol> repository, ILogger<RolData> logger)
            : base(repository, logger)
        {
        }

        // Aquí puedes agregar métodos específicos si son necesarios
    }
}