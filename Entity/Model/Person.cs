using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Person
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public DateTime DeleteAt { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
