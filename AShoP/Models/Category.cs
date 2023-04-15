namespace AShoP.Models;

public class Category
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public ICollection<Item>? Items { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category>? ChildCategories { get; set; }
}