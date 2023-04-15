using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AShoP.Controllers;

public class CategoriesAdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CategoriesAdmin
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Categories.Include(c => c.ParentCategory);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: CategoriesAdmin/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null || _context.Categories == null) return NotFound();

        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null) return NotFound();

        return View(category);
    }

    // GET: CategoriesAdmin/Create
    public IActionResult Create()
    {
        var data = new SelectList(_context.Categories, "Id", "Name");
        var list = data.ToList();
        list.Insert(0, new SelectListItem { Value = "", Text = "None" });
        ViewData["ParentCategoryId"] = list;
        return View();
    }

    // POST: CategoriesAdmin/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,ParentCategoryId")] Category category)
    {
        if (ModelState.IsValid)
        {
            category.Id = Guid.NewGuid();
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentCategoryId);
        return View(category);
    }

    // GET: CategoriesAdmin/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null || _context.Categories == null) return NotFound();

        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        var data = new SelectList(_context.Categories, "Id", "Name", category.ParentCategoryId);
        var list = data.ToList();
        list.Insert(0, new SelectListItem { Value = "", Text = "None" });
        ViewData["ParentCategoryId"] = list;
        return View(category);
    }

    // POST: CategoriesAdmin/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,ParentCategoryId")] Category category)
    {
        if (id != category.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Name", category.ParentCategoryId);
        return View(category);
    }

    // GET: CategoriesAdmin/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null || _context.Categories == null) return NotFound();

        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null) return NotFound();

        return View(category);
    }

    // POST: CategoriesAdmin/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (_context.Categories == null) return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        var category = await _context.Categories.FindAsync(id);
        if (category != null) _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(Guid id)
    {
        return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}