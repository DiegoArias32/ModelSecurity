using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class FormModule
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public required Module Module { get; set; }
        public int FormId { get; set; }
        public required Form Form { get; set; }
        public DateTime DeleteAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
