namespace AShoP.Models;

public class Stock
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Item? Item { get; set; }
    public int Quantity { get; set; }
}