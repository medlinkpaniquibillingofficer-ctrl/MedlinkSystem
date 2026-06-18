using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InventoryItemsController : Controller
    {
        private readonly AppDbContext _db;

        public InventoryItemsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /InventoryItems
        public async Task<IActionResult> Index(){
            var items = await _db.InventoryItems
                .Where(i => i.IsActive)
                .OrderBy(i => i.Name)
                .ToListAsync();
            return View(items);
        }

        // GET: /InventoryItems/Create
        public IActionResult Create() => View("CreateEdit", new InventoryItemModel());

        // POST: /InventoryItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryItemModel item)
        {
            if (ModelState.IsValid)
            {
                item.CreatedAt = DateTime.Now;
                _db.InventoryItems.Add(item);
                await _db.SaveChangesAsync();

                // Log starting stock as an Adjustment so history stays consistent from day one
                if (item.CurrentStock > 0)
                {
                    _db.StockTransactions.Add(new StockTransactionModel
                    {
                        InventoryItemId = item.Id,
                        TransactionDate = DateTime.Now,
                        TransactionType = TransactionType.Adjustment,
                        Quantity = item.CurrentStock,
                        Notes = "Initial stock on item creation"
                    });
                    await _db.SaveChangesAsync();
                }

                TempData["Success"] = "Item added successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View("CreateEdit", item);
        }

        // GET: /InventoryItems/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();
            return View("CreateEdit", item);
        }

        // POST: /InventoryItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryItemModel item)
        {
            if (id != item.Id) return NotFound();

            var existing = await _db.InventoryItems.FindAsync(id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                // Only descriptive fields are updated here — CurrentStock changes only through transactions
                existing.Name = item.Name;
                existing.Unit = item.Unit;
                existing.ReorderLevel = item.ReorderLevel;

                await _db.SaveChangesAsync();
                TempData["Success"] = "Item updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View("CreateEdit", item);
        }

        // GET: /InventoryItems/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /InventoryItems/Delete/5 (soft delete / archive)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _db.InventoryItems.FindAsync(id);
            if (item != null)
            {
                item.IsActive = false;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}