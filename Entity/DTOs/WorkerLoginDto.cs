using System;

namespace Entity.DTOs
{
    public class WorkerLoginDto
    {
        public int id { get; set; }
        public int LoginId { get; set; }
        public int WorkerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
    }

}
