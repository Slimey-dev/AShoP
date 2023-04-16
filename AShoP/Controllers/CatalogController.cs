using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AShoP.Controllers;

public class CatalogController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CatalogController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
        _context = context;
    }

    private Task<IdentityUser> GetCurrentUserAsync()
    {
        return _userManager.GetUserAsync(HttpContext.User);
    }
    // GET: Catalog

    public IActionResult Index(string? id)
    {
        if (User.Identity!.IsAuthenticated)
        {
            var haveCart = _context.Orders.Any(c => c.IsOrder == false);
            if (haveCart == false)
            {
                var guid = new Guid();
                var order = new Order
                {
                    Id = guid,
                    CustomerId = Guid.Parse(GetCurrentUserAsync().Result.Id),
                    Total = 0,
                    IsOrder = false
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
        }


        if (string.IsNullOrEmpty(id))
        {
            var topLevelCategories = _context.Categories.Where(c => c.ParentCategoryId == null).Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            ViewBag.Categories = topLevelCategories;
            var items = _context.Items.Select(i => new Item
            {
                Id = i.Id,
                Name = i.Name,
                Price = i.Price,
                Photo = i.Photo
            }).ToList();

            ViewBag.Items = items;
            return View();
        }
        else
        {
            var guid = Guid.Parse(id);
            var categories = _context.Categories.Where(c => c.ParentCategoryId == guid).Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            ViewBag.Categories = categories;

            var wantedCategories = categories.Select(c => c.Id).ToList();
            wantedCategories.Add(guid);

            var items = _context.Items.Where(i => wantedCategories.Contains((Guid)i.CategoryId))
                .Select(i => new Item
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    Photo = i.Photo
                }).ToList();

            var breadcrumbs = new List<Category>();
            var currentCategory = _context.Categories.Where(c => c.Id == guid).Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId
            }).First();
            breadcrumbs.Add(currentCategory);
            while (true)
                if (currentCategory.ParentCategoryId != null)
                {
                    currentCategory = _context.Categories.Where(c => c.Id == currentCategory.ParentCategoryId).Select(
                        c => new Category
                        {
                            Id = c.Id,
                            Name = c.Name,
                            ParentCategoryId = c.ParentCategoryId
                        }).First();
                    breadcrumbs.Add(currentCategory);
                }
                else
                {
                    break;
                }

            ViewBag.Breadcrumbs = breadcrumbs;
            ViewBag.Items = items;
            return View();
        }
    }
}