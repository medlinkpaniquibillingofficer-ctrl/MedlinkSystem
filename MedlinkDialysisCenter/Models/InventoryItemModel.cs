using System.ComponentModel.DataAnnotations;

namespace MedlinkDialysisCenter.Models
{
    public class InventoryItemModel
    {
        public int      Id              { get; set; }

        [Required, MaxLength(150)]
        public string   Name            { get; set; }

        [Required, MaxLength(30)]
        public string   Unit            { get; set; }

        public int      CurrentStock    { get; set; } = 0;

        public int      ReorderLevel    { get; set; } = 0;

        public bool     IsActive        { get; set; } = true;

        public DateTime CreatedAt       { get; set; } = DateTime.Now;

        public List<StockTransactionModel> StockTransactions { get; set; } = new();
    }
}