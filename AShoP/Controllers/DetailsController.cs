using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Mvc;

namespace AShoP.Controllers;

public class DetailsController : Controller
{
    private readonly ApplicationDbContext _context;

    public DetailsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET
    public IActionResult Index(string id)
    {
        if (string.IsNullOrEmpty(id)) return RedirectToPage("/");

        var guid = Guid.Parse(id);

        var item = _context.Items.Where(c => c.Id == guid).Select(i => new Item
        {
            Id = i.Id,
            Name = i.Name,
            Price = i.Price,
            Photo = i.Photo
        }).First();

        ViewBag.Item = item;

        if (ViewBag.Item == null) return NotFound();

        return View();
    }
}