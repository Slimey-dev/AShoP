using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AShoP.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
        _context = context;
    }

    private Task<IdentityUser> GetCurrentUserAsync()
    {
        return _userManager.GetUserAsync(HttpContext.User);
    }

    // GET
    public IActionResult Index()
    {
        if (!User.Identity!.IsAuthenticated) return Unauthorized();
        var userid = Guid.Parse(GetCurrentUserAsync().Result.Id);
        var order = _context.Orders.First(c => c.CustomerId == userid && c.IsOrder == false);
        var items = _context.OrderItems.Include(ci => ci.Item).Where(c => c.OrderId == order.Id).Select(c =>
            new OrderItem
            {
                Id = c.Id,
                ItemId = c.Id,
                Item = c.Item,
                Price = c.Price,
                Quantity = c.Quantity
            }).ToList();

        ViewBag.Cart = items;

        ViewBag.Total = RecalculateTotal();

        return View();
    }

    [HttpPost]
    public IActionResult GetItems(string id, int quantity)
    {
        var guid = Guid.Parse(id);
        var item = _context.Items.First(c => c.Id == guid);
        if (!User.Identity!.IsAuthenticated) return Unauthorized();
        OrderItem(item, quantity);
        return RedirectToAction("Index");
    }

    public void OrderItem(Item item, int quantity)
    {
        var guid = Guid.NewGuid();
        var userid = Guid.Parse(GetCurrentUserAsync().Result.Id);

        var orderid = _context.Orders.First(c => c.IsOrder == false && c.CustomerId == userid);
        var orderItem = new OrderItem
        {
            Id = guid,
            ItemId = item.Id,
            OrderId = orderid.Id,
            Price = item.Price,
            Quantity = quantity
        };
        _context.OrderItems.Add(orderItem);
        _context.SaveChanges();
    }

    public IActionResult CartChangeQuantity(string id, int quantity)
    {
        var guid = Guid.Parse(id);

        var orderItem = _context.OrderItems.First(c => c.Id == guid);
        orderItem.Quantity = quantity;
        _context.OrderItems.Update(orderItem);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult RemoveItem(string id, int quantity)
    {
        var guid = Guid.Parse(id);

        var orderItem = _context.OrderItems.First(c => c.Id == guid);
        _context.OrderItems.Remove(orderItem);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public decimal RecalculateTotal()
    {
        var userid = Guid.Parse(GetCurrentUserAsync().Result.Id);
        var order = _context.Orders.Include(c => c.OrderItems).First(c => c.CustomerId == userid && c.IsOrder == false);

        decimal total = 0;

        foreach (var item in order.OrderItems!) total += item.Price * item.Quantity;

        order.Total = total;

        _context.Orders.Update(order);
        _context.SaveChanges();

        return total;
    }
}