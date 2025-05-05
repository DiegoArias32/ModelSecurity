using System;

namespace Entity.Model
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }  // Create, Update, Delete, PermanentDelete, etc.
        public string EntityType { get; set; }  // User, Role, Form, etc.
        public string EntityId { get; set; }
        public string Details { get; set; }  // Detalles adicionales en formato JSON
    }
}