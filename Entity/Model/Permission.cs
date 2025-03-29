using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Permission
    {
        public int Id { get; set; }

        public required string Can_Read { get; set; }
        public required string Can_Create { get; set;}
        public required string Can_Update { get; set;}
        public required string Can_Delete { get; set;}

        public DateTime CreateAt { get; set; }
    }
}
