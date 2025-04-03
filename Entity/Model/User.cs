using System;
using Microsoft.VisualBasic;

namespace Entity.Model
{
    public class User
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime CreateAt { get; set; } 
        public DateTime DeleteAt { get; set; }

    }
}
