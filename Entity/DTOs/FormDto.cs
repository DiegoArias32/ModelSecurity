using System;

namespace Entity.DTOs
{
    public class FormDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public bool Active { get; set; }
    }
}
