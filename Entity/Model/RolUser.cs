using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    internal class RolUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public int RolId { get; set; }
        public required Rol Rol { get; set; }
        public DateTime DeleteAt { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
