namespace AShoP.Models;

public class Discount
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Item? Item { get; set; }
    public int Quantity { get; set; }
    public decimal Sale { get; set; }
    public bool IsPercent { get; set; }
}