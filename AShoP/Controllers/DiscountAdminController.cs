using AShoP.Data;
using AShoP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AShoP.Controllers;

[Authorize(Roles = "Administrator")]
public class DiscountAdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public DiscountAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: DiscountAdmin
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Discounts.Include(d => d.Item);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: DiscountAdmin/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null || _context.Discounts == null) return NotFound();

        var discount = await _context.Discounts
            .Include(d => d.Item)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (discount == null) return NotFound();

        return View(discount);
    }

    // GET: DiscountAdmin/Create
    public IActionResult Create()
    {
        ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name");
        return View();
    }

    // POST: DiscountAdmin/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ItemId,Quantity,Sale,IsPercent")] Discount discount)
    {
        if (ModelState.IsValid)
        {
            discount.Id = Guid.NewGuid();
            _context.Add(discount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", discount.ItemId);
        return View(discount);
    }

    // GET: DiscountAdmin/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null || _context.Discounts == null) return NotFound();

        var discount = await _context.Discounts.FindAsync(id);
        if (discount == null) return NotFound();
        ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", discount.ItemId);
        return View(discount);
    }

    // POST: DiscountAdmin/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,ItemId,Quantity,Sale,IsPercent")] Discount discount)
    {
        if (id != discount.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(discount);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountExists(discount.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", discount.ItemId);
        return View(discount);
    }

    // GET: DiscountAdmin/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null || _context.Discounts == null) return NotFound();

        var discount = await _context.Discounts
            .Include(d => d.Item)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (discount == null) return NotFound();

        return View(discount);
    }

    // POST: DiscountAdmin/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (_context.Discounts == null) return Problem("Entity set 'ApplicationDbContext.Discounts'  is null.");
        var discount = await _context.Discounts.FindAsync(id);
        if (discount != null) _context.Discounts.Remove(discount);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DiscountExists(Guid id)
    {
        return (_context.Discounts?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}