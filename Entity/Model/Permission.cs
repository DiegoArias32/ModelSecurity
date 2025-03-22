using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Permission
    {
        public int Id { get; set; }

        public string Can_Read { get; set; }
        public string Can_Create { get; set;}
        public string Can_Update { get; set;}
        public string Can_Delete { get; set;}

        public DateTime CreateAt { get; set; }
    }
}
