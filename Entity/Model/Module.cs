using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Module
    {

        public int Id { get; set; }
        public required string Code { get; set; }
        public bool Active { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }

    }
}
