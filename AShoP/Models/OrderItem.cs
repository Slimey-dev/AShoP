namespace AShoP.Models;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Guid ItemId { get; set; }
    public Item? Item { get; set; }
}