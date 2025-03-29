using System;

namespace Entity.DTOs
{
    public class WorkerLoginDto
    {
        public int LoginId { get; set; }
        public int WorkerId { get; set; }
        public required string Username { get; set; }
        public DateTime CreationDate { get; set; }
        public required string Status { get; set; }
    }

}
