using System;

namespace Entity.DTOs
{
    public class PersonDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
