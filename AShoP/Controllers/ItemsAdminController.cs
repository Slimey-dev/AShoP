using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AShoP.Controllers;

public class ItemsAdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public ItemsAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ItemsAdmin
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Items.Include(i => i.Category);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: ItemsAdmin/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null || _context.Items == null) return NotFound();

        var item = await _context.Items
            .Include(i => i.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: ItemsAdmin/Create
    public IActionResult Create()
    {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        return View();
    }

    // POST: ItemsAdmin/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,Name,Price,VAT,EndedDate,CategoryId,SaleForQuantity,QuantityForSale,DaysForRestock,Origin,Photo")]
        Item item)
    {
        if (ModelState.IsValid)
        {
            item.Id = Guid.NewGuid();
            _context.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", item.CategoryId);
        return View(item);
    }

    // GET: ItemsAdmin/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null || _context.Items == null) return NotFound();

        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", item.CategoryId);
        return View(item);
    }

    // POST: ItemsAdmin/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id,
        [Bind("Id,Name,Price,VAT,EndedDate,CategoryId,SaleForQuantity,QuantityForSale,DaysForRestock,Origin,Photo")]
        Item item)
    {
        if (id != item.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(item.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", item.CategoryId);
        return View(item);
    }

    // GET: ItemsAdmin/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null || _context.Items == null) return NotFound();

        var item = await _context.Items
            .Include(i => i.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: ItemsAdmin/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (_context.Items == null) return Problem("Entity set 'ApplicationDbContext.Items'  is null.");
        var item = await _context.Items.FindAsync(id);
        if (item != null) _context.Items.Remove(item);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ItemExists(Guid id)
    {
        return (_context.Items?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}