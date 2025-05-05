using System;
using Microsoft.VisualBasic;

namespace Entity.Model
{
    public class User
    {
        public int Id { get; set; }
        public int? WorkerId { get; set; }  // Cambié PersonId a WorkerId
        public Worker? Worker { get; set; }  // Cambié Person a Worker
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }
}
