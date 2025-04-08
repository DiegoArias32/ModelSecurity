using System;

namespace Entity.Model
{
    public class Form
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public bool Active { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }

    }
}