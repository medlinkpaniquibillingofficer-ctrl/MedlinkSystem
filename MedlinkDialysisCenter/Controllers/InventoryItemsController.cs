using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

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

        // GET: /InventoryItems/ExportToExcel
        public async Task<IActionResult> ExportToExcel()
        {
            var items = await _db.InventoryItems
                .Where(i => i.IsActive)
                .OrderBy(i => i.Name)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Inventory Report");

            // ── Styles ──────────────────────────────────────────────
            var headerFill = XLColor.FromArgb(0x1F, 0x49, 0x7D);   // dark blue
            var lowStockFill = XLColor.FromArgb(0xFF, 0xE6, 0x99);    // amber

            // ── Title block ─────────────────────────────────────────
            ws.Cell("A1").Value = "Medlink Dialysis Center – Inventory Report";
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Range("A1:G1").Merge();

            ws.Cell("A2").Value = $"Generated: {DateTime.Now:MMMM dd, yyyy  hh:mm tt}";
            ws.Cell("A2").Style.Font.Italic = true;
            ws.Range("A2:G2").Merge();

            // ── Summary row ─────────────────────────────────────────
            int totalItems = items.Count;
            int lowStockCount = items.Count(i => i.CurrentStock <= i.ReorderLevel);

            ws.Cell("A3").Value = $"Total Active Items: {totalItems}     |     Items at/below Reorder Level: {lowStockCount}";
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Range("A3:G3").Merge();

            // ── Column headers (row 5) ───────────────────────────────
            int headerRow = 5;
            string[] headers = { "#", "Item Name", "Unit", "Current Stock", "Reorder Level", "Status", "Created" };

            for (int col = 1; col <= headers.Length; col++)
            {
                var cell = ws.Cell(headerRow, col);
                cell.Value = headers[col - 1];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = headerFill;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // ── Data rows ────────────────────────────────────────────
            int dataStartRow = headerRow + 1;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int row = dataStartRow + i;
                bool isLow = item.CurrentStock <= item.ReorderLevel;
                string status = item.CurrentStock == 0 ? "Out of Stock"
                              : isLow ? "Low Stock"
                                                    : "OK";

                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = item.Name;
                ws.Cell(row, 3).Value = item.Unit;
                ws.Cell(row, 4).Value = item.CurrentStock;
                ws.Cell(row, 5).Value = item.ReorderLevel;
                ws.Cell(row, 6).Value = status;
                ws.Cell(row, 7).Value = item.CreatedAt.ToString("MM/dd/yyyy");

                // Highlight low-stock rows
                if (isLow)
                {
                    ws.Range(row, 1, row, 7).Style.Fill.BackgroundColor = lowStockFill;
                }

                // Alternate row shading for non-highlighted rows
                if (!isLow && i % 2 == 1)
                {
                    ws.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(0xF2, 0xF2, 0xF2);
                }

                // Borders
                ws.Range(row, 1, row, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(row, 1, row, 7).Style.Border.InsideBorder = XLBorderStyleValues.Hair;

                // Center numeric columns
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // ── Legend ───────────────────────────────────────────────
            int legendRow = dataStartRow + items.Count + 2;
            ws.Cell(legendRow, 1).Value = "Legend:";
            ws.Cell(legendRow, 1).Style.Font.Bold = true;

            ws.Cell(legendRow + 1, 1).Value = "Amber rows = stock is at or below reorder level";
            ws.Cell(legendRow + 1, 1).Style.Fill.BackgroundColor = lowStockFill;
            ws.Range(legendRow + 1, 1, legendRow + 1, 3).Merge();

            // ── Column widths ────────────────────────────────────────
            ws.Column(1).Width = 5;
            ws.Column(2).Width = 30;
            ws.Column(3).Width = 12;
            ws.Column(4).Width = 16;
            ws.Column(5).Width = 16;
            ws.Column(6).Width = 14;
            ws.Column(7).Width = 14;

            // Freeze the header row
            ws.SheetView.FreezeRows(headerRow);

            // ── Stream to browser ────────────────────────────────────
            string fileName = $"Inventory_Report_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

    }
}