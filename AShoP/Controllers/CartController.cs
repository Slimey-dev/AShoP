using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
}