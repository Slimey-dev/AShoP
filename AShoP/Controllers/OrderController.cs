using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AShoP.Controllers;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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
        var customer = _context.Customers.First(c => c.Id == userid);
        ViewBag.Customer = customer;
        return View();
    }

    [HttpPost]
    public IActionResult Order(string name, string phone, string address, string city, string state, string zip,
        string country, bool save)
    {
        if (!User.Identity!.IsAuthenticated) return Unauthorized();

        var userid = Guid.Parse(GetCurrentUserAsync().Result.Id);
        var customer = _context.Customers.First(c => c.Id == userid);

        if (save)
        {
            customer.Name = name;
            customer.Address = address;
            customer.Phone = phone;
            customer.City = city;
            customer.State = state;
            customer.Zip = zip;
            customer.Country = country;

            _context.Customers.Update(customer);
            _context.SaveChanges();
        }

        var order = _context.Orders.First(c => c.IsOrder == false);
        order.Address = address;
        order.City = city;
        order.Country = country;
        order.State = state;
        order.Customer = customer;
        order.Zip = zip;
        order.IsOrder = true;
        _context.Orders.Update(order);
        _context.SaveChanges();

        if (User.Identity!.IsAuthenticated)
        {
            var haveCart = _context.Orders.Any(c => c.IsOrder == false);
            if (haveCart == false)
            {
                var guid = new Guid();
                var neworder = new Order
                {
                    Id = guid,
                    CustomerId = Guid.Parse(GetCurrentUserAsync().Result.Id),
                    Total = 0,
                    IsOrder = false
                };
                _context.Orders.Add(neworder);
                _context.SaveChanges();
            }
        }

        RecalculateTotal();

        return View();
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