namespace Entity.DTOs
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public List<MenuItemDto> Children { get; set; } = new List<MenuItemDto>();
    }
}