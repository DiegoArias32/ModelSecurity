using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Rol
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public DateTime CreateAt { get; set; }
        public bool Active { get; set; }

    }
}
