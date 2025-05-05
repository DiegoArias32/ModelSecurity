using System;

namespace Entity.DTOs
{
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Details { get; set; }
    }
}