using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AShoP.Controllers;

public class StockAdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public StockAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: StockAdmin
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Stock.Include(s => s.Item);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: StockAdmin/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null || _context.Stock == null) return NotFound();

        var stock = await _context.Stock
            .Include(s => s.Item)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (stock == null) return NotFound();

        return View(stock);
    }

    // GET: StockAdmin/Create
    public IActionResult Create()
    {
        ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name");
        return View();
    }

    // POST: StockAdmin/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ItemId,Quantity")] Stock stock)
    {
        if (ModelState.IsValid)
        {
            stock.Id = Guid.NewGuid();
            _context.Add(stock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", stock.ItemId);
        return View(stock);
    }

    // GET: StockAdmin/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null || _context.Stock == null) return NotFound();

        var stock = await _context.Stock.FindAsync(id);
        if (stock == null) return NotFound();
        ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", stock.ItemId);
        return View(stock);
    }

    // POST: StockAdmin/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,ItemId,Quantity")] Stock stock)
    {
        if (id != stock.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(stock);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(stock.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", stock.ItemId);
        return View(stock);
    }

    // GET: StockAdmin/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null || _context.Stock == null) return NotFound();

        var stock = await _context.Stock
            .Include(s => s.Item)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (stock == null) return NotFound();

        return View(stock);
    }

    // POST: StockAdmin/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (_context.Stock == null) return Problem("Entity set 'ApplicationDbContext.Stock'  is null.");
        var stock = await _context.Stock.FindAsync(id);
        if (stock != null) _context.Stock.Remove(stock);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StockExists(Guid id)
    {
        return (_context.Stock?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}