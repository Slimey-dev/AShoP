using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Mvc;

namespace AShoP.Controllers;

public class CatalogController : Controller
{
    private readonly ApplicationDbContext _context;

    public CatalogController(ApplicationDbContext context)
    {
        _context = context;
    }
    // GET: Catalog

    public IActionResult Index(string? id)
    {
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