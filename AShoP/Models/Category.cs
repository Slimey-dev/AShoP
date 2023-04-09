namespace AShoP.Models;

public class Category
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public ICollection<Item>? Items { get; set; }
    public ICollection<Category>? SubCategories { get; set; }
}