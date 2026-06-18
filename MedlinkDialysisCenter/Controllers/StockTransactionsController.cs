using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;
using MedlinkDialysisCenter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Controllers
{
    [Authorize(Roles = "Admin,Nurse")]
    public class StockTransactionsController : Controller
    {
        private readonly AppDbContext _db;

        public StockTransactionsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /StockTransactions/Create?itemId=5
        public async Task<IActionResult> Create(int itemId)
        {
            var item = await _db.InventoryItems.FindAsync(itemId);
            if (item == null) return NotFound();

            var model = new StockTransactionFormViewModel
            {
                InventoryItemId = item.Id,
                ItemName = item.Name,
                Unit = item.Unit,
                CurrentStock = item.CurrentStock,
                TransactionDate = DateTime.Now
            };

            return View(model);
        }

        // POST: /StockTransactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransactionFormViewModel model)
        {
            var item = await _db.InventoryItems.FindAsync(model.InventoryItemId);
            if (item == null) return NotFound();

            // Re-populate display fields in case we need to redisplay the form
            model.ItemName = item.Name;
            model.Unit = item.Unit;
            model.CurrentStock = item.CurrentStock;

            int stockDelta;
            int storedQuantity = model.Quantity;

            switch (model.TransactionType)
            {
                case TransactionType.Usage:
                    stockDelta = -model.Quantity;
                    break;
                case TransactionType.Received:
                    stockDelta = model.Quantity;
                    break;
                case TransactionType.Adjustment:
                    stockDelta = model.AdjustmentDirection == AdjustmentDirection.Increase
                        ? model.Quantity
                        : -model.Quantity;
                    storedQuantity = stockDelta; // store signed value so history shows direction
                    break;
                default:
                    stockDelta = 0;
                    break;
            }

            if (ModelState.IsValid && item.CurrentStock + stockDelta < 0)
            {
                ModelState.AddModelError(nameof(model.Quantity),
                    $"This would result in negative stock. Current stock is {item.CurrentStock} {item.Unit}.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            item.CurrentStock += stockDelta;

            _db.StockTransactions.Add(new StockTransactionModel
            {
                InventoryItemId     = item.Id,
                TransactionDate     = model.TransactionDate,
                TransactionType     = model.TransactionType,
                Quantity            = storedQuantity,
                Notes               = model.Notes,
                CreatedAt           = DateTime.Now
            });

            await _db.SaveChangesAsync();

            TempData["Success"] = $"{model.TransactionType} recorded for {item.Name}.";
            return RedirectToAction("Index", "InventoryItems");
        }

        public async Task<IActionResult> History(int itemId){
            var item = await _db.InventoryItems.FindAsync(itemId);
            if (item == null) return NotFound();

            var transactions = await _db.StockTransactions
                .Where(t => t.InventoryItemId == itemId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            ViewBag.Item = item;
            return View(transactions);
        }
    }
}