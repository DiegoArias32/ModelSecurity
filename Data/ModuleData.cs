// Data/ModuleData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;
using Module = Entity.Model.Module;

namespace Data
{
    public class ModuleData : BaseData<Module>
    {
        public ModuleData(IRepository<Module> repository, ILogger<ModuleData> logger)
            : base(repository, logger)
        {
        }

        // Si necesitas sobrescribir algún método o agregar métodos específicos, puedes hacerlo aquí
    }
}