using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RolFormPermission
    {
        public int Id { get; set; }
        public int RolId { get; set; }
        public required Rol Rol { get; set; }
        public int FormId { get; set; }
        public required Form Form { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime DeleteAt { get; set; }

    }
}
