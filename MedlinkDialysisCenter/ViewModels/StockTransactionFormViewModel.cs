using MedlinkDialysisCenter.Models;
using System.ComponentModel.DataAnnotations;

namespace MedlinkDialysisCenter.ViewModels
{
    public enum AdjustmentDirection
    {
        Increase,
        Decrease
    }

    public class StockTransactionFormViewModel
    {
        public int      InventoryItemId     { get; set; }
        public string   ItemName            { get; set; } = string.Empty;
        public string   Unit                { get; set; } = string.Empty;
        public int      CurrentStock        { get; set; }

        [Required]
        public TransactionType      TransactionType     { get; set; } = TransactionType.Usage;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int                  Quantity            { get; set; }

        public AdjustmentDirection  AdjustmentDirection { get; set; } = AdjustmentDirection.Increase;

        [Required]
        public DateTime             TransactionDate     { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string?              Notes               { get; set; }
    }
}