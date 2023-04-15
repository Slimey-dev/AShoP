namespace AShoP.Models;

public class Item
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public decimal VAT { get; set; }
    public DateTimeOffset EndedDate { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public decimal SaleForQuantity { get; set; }
    public int QuantityForSale { get; set; }
    public int DaysForRestock { get; set; }
    public string? Origin { get; set; }
    public string? Photo { get; set; }
}